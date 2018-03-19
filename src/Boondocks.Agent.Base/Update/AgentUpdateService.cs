namespace Boondocks.Agent.Base.Update
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Interfaces;
    using Serilog;
    using Services.Device.Contracts;
    using Services.Device.WebApiClient;

    internal class AgentUpdateService : UpdateService
    {
        private readonly IDockerClient _dockerClient;
        private readonly AgentDockerContainerFactory _dockerContainerFactory;
        private readonly DeviceApiClient _deviceApiClient;
        private readonly IPlatformDetector _platformDetecter;

        public AgentUpdateService(
            IDockerClient dockerClient,
            AgentDockerContainerFactory dockerContainerFactory,
            DeviceApiClient deviceApiClient,
            IPlatformDetector platformDetecter,
            ILogger logger) : base(logger, dockerClient)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _dockerContainerFactory = dockerContainerFactory ?? throw new ArgumentNullException(nameof(dockerContainerFactory));
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
            _platformDetecter = platformDetecter;
        }

        public async Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var container = await _dockerClient.GetContainerByName(DockerConstants.AgentContainerName, cancellationToken);

            return container?.ImageID;
        }

        /// <summary>
        /// If true, the caller should exit
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<bool> UpdateCoreAsync(GetDeviceConfigurationResponse configuration, CancellationToken cancellationToken)
        {
            string currentVersion = await GetCurrentVersionAsync(cancellationToken);

            VersionReference nextVersion = configuration.AgentVersion;

            if (nextVersion == null)
                return false;

            if (currentVersion == nextVersion.ImageId)
                return false;

            //We don't support updating on linux
            if (!_platformDetecter.IsLinux)
                return false;

            var renameParameters = new ContainerRenameParameters()
            {
                NewName = DockerConstants.AgentContainerOutgoingName
            };

            //Remove the outgoing application (just in case)
            await _dockerClient.ObliterateContainerAsync(DockerConstants.AgentContainerOutgoingName, Logger, cancellationToken);

            //Get the existing container
            var existingContainer = await _dockerClient.GetContainerByName(DockerConstants.AgentContainerName, cancellationToken);

            //Rename to a temporary container name
            if (existingContainer != null)
            {
                await _dockerClient.Containers.RenameContainerAsync(existingContainer.ID, renameParameters, cancellationToken);
            }

            //Create the new updated container
            var createContainerResponse = await _dockerContainerFactory.CreateContainerForUpdateAsync(
                _dockerClient,
                nextVersion.ImageId,
                cancellationToken);

            //Show the warnings
            if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
            {
                string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                Logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
            }

            Logger.Information("Container {ContainerId} created for agent {ImageId}. Starting...", createContainerResponse.ID, nextVersion.ImageId);

            //Attempt to start the container
            var started = await _dockerClient.Containers.StartContainerAsync(
                createContainerResponse.ID,
                new ContainerStartParameters(),
                cancellationToken);

            //Check to see if the application started
            if (started)
            {
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
                    
            Logger.Warning("Warning: Agent not started.");

            return false;
        }
    }
}