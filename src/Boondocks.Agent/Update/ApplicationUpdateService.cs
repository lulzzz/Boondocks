namespace Boondocks.Agent.Update
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
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
            ILogger logger) : base(logger)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _dockerContainerFactory = dockerContainerFactory ?? throw new ArgumentNullException(nameof(dockerContainerFactory));
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
        }

        public override async Task<string> GetCurrentVersionAsync()
        {
            var container = await _dockerClient.GetContainerByName(DockerConstants.ApplicationContainerName, new CancellationToken());

            return container?.ImageID;
        }

        public override VersionReference GetVersionFromConfiguration(GetDeviceConfigurationResponse response)
        {
            return response?.ApplicationVersion;
        }

        public override async Task<bool> UpdateCoreAsync(VersionReference version, CancellationToken cancellationToken)
        {   
            //It shouldn't already be downloaded, but check anyway.
            if (!await _dockerClient.DoesImageExistAsync(version.ImageId, cancellationToken))
            {
                //Download the application
                await DownloadApplicationImageAsync(_dockerClient, version, cancellationToken);
            }

            //Ditch the current applications
            await StopAndDestroyApplicationAsync(_dockerClient, DockerConstants.ApplicationContainerName, cancellationToken);

            Logger.Information("Create the container for {ImageId} ...", version.ImageId);

            //Create the container
            var createContainerResponse = await _dockerContainerFactory.CreateContainerAsync(
                _dockerClient,
                    version.ImageId,
                    cancellationToken);

            if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
            {
                string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                Logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
            }

            Logger.Information("Container {ContainerId} created for application {}. Starting...", createContainerResponse.ID, version.ImageId);

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

        private async Task DownloadApplicationImageAsync(IDockerClient dockerClient, VersionReference versionReference,
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