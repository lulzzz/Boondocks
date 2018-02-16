namespace Boondocks.Agent
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Interfaces;
    using Model;
    using Serilog;
    using Services.Device.Contracts;
    using Services.Device.WebApiClient;


    internal class ApplicationUpdateService : UpdateService
    {
        private readonly IDockerClient _dockerClient;
        private readonly OperationalStateProvider _operationalStateProvider;
        private readonly ApplicationDockerContainerFactory _dockerContainerFactory;
        private readonly DeviceApiClient _deviceApiClient;

        public ApplicationUpdateService(
            IDockerClient dockerClient,
            OperationalStateProvider operationalStateProvider,
            ApplicationDockerContainerFactory dockerContainerFactory,
            DeviceApiClient deviceApiClient,
            ILogger logger) : base(logger)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _operationalStateProvider = operationalStateProvider ?? throw new ArgumentNullException(nameof(operationalStateProvider));
            _dockerContainerFactory = dockerContainerFactory ?? throw new ArgumentNullException(nameof(dockerContainerFactory));
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
        }

        public override bool IsUpdatePending()
        {
            return _operationalStateProvider.State.NextApplicationVersion != null;
        }

        public override async Task<bool> UpdateCoreAsync(CancellationToken cancellationToken)
        {
            
            string imageId = _operationalStateProvider.State.NextApplicationVersion.ImageId;

                    
            //It shouldn't already be downloaded, but check anyway.
            if (!await _dockerClient.DoesImageExistAsync(imageId, cancellationToken))
            {
                //Download the application
                await DownloadApplicationImageAsync(_dockerClient, _operationalStateProvider.State.NextApplicationVersion, cancellationToken);
            }

            //Ditch the current applications
            await StopAndDestroyApplicationAsync(_dockerClient, DockerConstants.ApplicationContainerName, cancellationToken);

            Logger.Information("Create the container for {ImageId} ...", imageId);

            //Create the container
            var createContainerResponse = await _dockerContainerFactory.CreateContainerAsync(
                _dockerClient,
                    imageId,
                    cancellationToken);

            if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
            {
                string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                Logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
            }

            Logger.Information("Container {ContainerId} created for application {}. Starting...", createContainerResponse.ID, imageId);

            //Attempt to start the container
            var started = await _dockerClient.Containers.StartContainerAsync(
                createContainerResponse.ID,
                new ContainerStartParameters(),
                cancellationToken);

            //Check to see if the application started
            if (started)
            {
                //Update the operational state
                _operationalStateProvider.State.CurrentApplicationVersion = _operationalStateProvider.State.NextApplicationVersion;
                _operationalStateProvider.State.NextApplicationVersion = null;
                _operationalStateProvider.Save();
            }
            else
            {
                Logger.Warning("Warning: Application not started.");
            }   

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