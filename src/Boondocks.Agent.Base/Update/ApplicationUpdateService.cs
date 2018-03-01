namespace Boondocks.Agent.Base.Update
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Domain;
    using Model;
    using Serilog;
    using Services.Device.Contracts;
    using Services.Device.WebApiClient;

    internal class ApplicationUpdateService : UpdateService
    {
        private readonly IDockerClient _dockerClient;
        private readonly ApplicationDockerContainerFactory _dockerContainerFactory;
        private readonly DeviceApiClient _deviceApiClient;

        public ApplicationUpdateService(
            IDockerClient dockerClient,
            ApplicationDockerContainerFactory dockerContainerFactory,
            DeviceApiClient deviceApiClient,
            ILogger logger) : base(logger, dockerClient)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _dockerContainerFactory = dockerContainerFactory ?? throw new ArgumentNullException(nameof(dockerContainerFactory));
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
        }

        public async Task<string> GetCurrentVersionAsync()
        {
            var container = await _dockerClient.GetContainerByName(DockerConstants.ApplicationContainerName, new CancellationToken());

            return container?.ImageID;
        }

        public override async Task<bool> UpdateCoreAsync(GetDeviceConfigurationResponse configuration, CancellationToken cancellationToken)
        {
            //Get the current and next version
            string currentVersion = await GetCurrentVersionAsync();
            VersionReference nextVersion = configuration.ApplicationVersion;

            if (nextVersion == null)
                return false;

            bool versionChanged = currentVersion != nextVersion.ImageId;

            if (versionChanged)
            {
                Logger.Information($"The application version has changed from {currentVersion} to {nextVersion.ImageId}.");
            }
            else
            {
                Logger.Verbose($"The application version has stayed {currentVersion}.");
            }

            if (!versionChanged)
            {
                //Get the container
                var container = await _dockerClient.GetContainerByImageId(nextVersion.ImageId, cancellationToken);

                if (container != null)
                {
                    //Inspect the container to get its environment variables
                    var inspection =
                        await _dockerClient.Containers.InspectContainerAsync(container.ID, cancellationToken);

                    var comparer =
                        new EnvironmentVariableComparer(ApplicationDockerContainerFactory.ReservedEnvironmentVariables);

                    if (comparer.AreSame(inspection.Config.Env, configuration.EnvironmentVariables))
                    {
                        Logger.Verbose("The environment variables have stayed the same.");
                        return false;
                    }

                    Logger.Information("The environment variables have changed.");
                }
            }

            //Make sure that the image is downloaded
            if (!await _dockerClient.DoesImageExistAsync(nextVersion.ImageId, cancellationToken))
            {
                //Download the application
                await DownloadImageAsync(_dockerClient, nextVersion, cancellationToken);
            }

            //Ditch the current applications
            await _dockerClient.ObliterateContainerAsync(DockerConstants.ApplicationContainerName, Logger, cancellationToken);

            Logger.Information("Create the container for {ImageId} ...", nextVersion.ImageId);

            //Create the container
            var createContainerResponse = await _dockerContainerFactory.CreateContainerAsync(
                _dockerClient,
                nextVersion.ImageId,
                configuration.EnvironmentVariables,
                cancellationToken);

            if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
            {
                string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                Logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
            }

            Logger.Information("Container {ContainerId} created for application {Application}. Starting...", createContainerResponse.ID, nextVersion.ImageId);

            //Attempt to start the container
            var started = await _dockerClient.Containers.StartContainerAsync(
                createContainerResponse.ID,
                new ContainerStartParameters(),
                cancellationToken);

            //Check to see if the application started
            if (!started)
            {
                Logger.Warning("Warning: Application not started.");
            }   

            //We never need to exit the agent for updating an application.
            return false;
        }

        private async Task DownloadImageAsync(IDockerClient dockerClient, VersionReference versionReference,
            CancellationToken cancellationToken)
        {
            var versionRequest = new GetImageDownloadInfoRequest()
            {
                Id = versionReference.Id
            };

            Logger.Information("Getting application download information for version {ImageId}...", versionReference.ImageId);

            //Get the download info
            var downloadInfo =
                await _deviceApiClient.ApplicationDownloadInfo.GetApplicationVersionDownloadInfo(versionRequest,
                    cancellationToken);

            string fromImage = $"{downloadInfo.Registry}/{downloadInfo.Repository}:{downloadInfo.Name}";

            //Dowlnoad it!
            Logger.Information("Downloading with fromImage = '{FromImage}'...", fromImage);

            var imageCreateParameters = new ImagesCreateParameters
            {
                FromImage = fromImage
            };

            var authConfig = new AuthConfig()
            {

            };

            //Do the donwload!!!!!
            await dockerClient.Images.CreateImageAsync(
                imageCreateParameters,
                authConfig,
                new Progress<JSONMessage>(m => Console.WriteLine($"\tCreateImageProgress: {m.ProgressMessage}")),
                cancellationToken);

            Logger.Information("Application image {ImageId} downloaded.", versionReference.ImageId);
        }
    }


}