namespace Boondocks.Agent
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

    internal class AgentUpdateService : UpdateService
    {
        private readonly IDockerClient _dockerClient;
        private readonly OperationalStateProvider _operationalStateProvider;
        private readonly AgentDockerContainerFactory _dockerContainerFactory;
        private readonly DeviceApiClient _deviceApiClient;

        public AgentUpdateService(
            IDockerClient dockerClient,
            OperationalStateProvider operationalStateProvider,
            AgentDockerContainerFactory dockerContainerFactory,
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
            return _operationalStateProvider.State.NextAgentVersion != null;
        }

        /// <summary>
        /// If true, the caller should exit
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateCoreAsync(CancellationToken cancellationToken)
        {
            string imageId = _operationalStateProvider.State.NextAgentVersion.ImageId;

            //It shouldn't already be downloaded, but check anyway.
            if (!await _dockerClient.DoesImageExistAsync(imageId, cancellationToken))
            {
                //Download the application
                await DownloadSupervisorImageAsync(_dockerClient, _operationalStateProvider.State.NextAgentVersion, cancellationToken);
            }

            var renameParameters = new ContainerRenameParameters()
            {
                NewName = DockerConstants.AgentContainerOutgoingName
            };

            //TODO: Check for the existence of an outgoing agent container (and destroy it????)

            var existingContainer = await _dockerClient.GetContainerByName(DockerConstants.AgentContainerName, cancellationToken);

            if (existingContainer != null)
            {
                await _dockerClient.Containers.RenameContainerAsync(existingContainer.ID, renameParameters, cancellationToken);
            }

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

            Logger.Information("Container {ContainerId} created for agent {ImageId}. Starting...", createContainerResponse.ID, imageId);

            //Attempt to start the container
            var started = await _dockerClient.Containers.StartContainerAsync(
                createContainerResponse.ID,
                new ContainerStartParameters(),
                cancellationToken);

            //Check to see if the application started
            if (started)
            {
                //Update the operational state
                _operationalStateProvider.State.CurrentAgentVersion = _operationalStateProvider.State.NextAgentVersion;
                _operationalStateProvider.State.NextAgentVersion = null;
                _operationalStateProvider.Save();

                //Wait just a little bit to give the save time to complete.
                await Task.Delay(TimeSpan.FromSeconds(2), new CancellationToken());

                //Commit container suicide (this should kill the container that we're in)
                if (existingContainer != null)
                {
                    await _dockerClient.Containers.RemoveContainerAsync(existingContainer.ID,
                        new ContainerRemoveParameters()
                        {
                            Force = true
                        }, cancellationToken);
                }

                //We probably won't even get here
                return true;
            }
                    
            Logger.Warning("Warning: Supervisor not started.");

            return false;
        }

        private async Task DownloadSupervisorImageAsync(IDockerClient dockerClient, VersionReference versionReference, CancellationToken cancellationToken)
        {
            var versionRequest = new GetImageDownloadInfoRequest()
            {
                Id = versionReference.Id
            };

            Logger.Information("Getting supervisor download information for version {ImageId}...", versionReference.ImageId);

            //Get the download info
            var downloadInfo =
                await _deviceApiClient.SupervisorDownloadInfo.GetSupervisorVersionDownloadInfo(versionRequest,
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

            Logger.Information("Supervisor image {ImageId} downloaded.", versionReference.ImageId);
        }
    }
}