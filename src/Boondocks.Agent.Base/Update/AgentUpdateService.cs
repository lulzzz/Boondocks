namespace Boondocks.Agent.Base.Update
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

    internal class AgentUpdateService : UpdateService
    {
        private readonly IDockerClient _dockerClient;
        private readonly AgentDockerContainerFactory _dockerContainerFactory;
        private readonly IPlatformDetector _platformDetecter;

        public AgentUpdateService(
            IDockerClient dockerClient,
            AgentDockerContainerFactory dockerContainerFactory,
            IPlatformDetector platformDetecter,
            ILogger logger) : base(logger, dockerClient)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _dockerContainerFactory = dockerContainerFactory ?? throw new ArgumentNullException(nameof(dockerContainerFactory));
            _platformDetecter = platformDetecter;
        }

        public async Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var container = await _dockerClient.GetContainerByName(DockerContainerNames.Agent, cancellationToken);

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
            {
                Logger.Warning("Agent update is only supported on Linux.");
                return false;
            }

            //Get the existing container
            var existingContainer = await _dockerClient.GetContainerByName(DockerContainerNames.Agent, cancellationToken);

            if (existingContainer == null)
            {
                Logger.Error("Unable to find the existing container {ContainerName}", DockerContainerNames.Agent);
                throw new Exception("Unable to find the running agent container.");
            }

            //Remove the outgoing / incoming agent (just in case)
            await _dockerClient.ObliterateContainerAsync(DockerContainerNames.AgentOutgoing, Logger, cancellationToken);
            await _dockerClient.ObliterateContainerAsync(DockerContainerNames.AgentIncoming, Logger, cancellationToken);

            //Create the new updated container
            var createContainerResponse = await _dockerContainerFactory.CreateContainerForUpdateAsync(
                _dockerClient,
                nextVersion.ImageId,
                DockerContainerNames.Agent,
                DockerContainerNames.AgentIncoming,
                cancellationToken);

            //Show the warnings
            if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
            {
                string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                Logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
            }

            Logger.Information("Container {ContainerId} created for agent {ImageId}. Starting...", createContainerResponse.ID, nextVersion.ImageId);

            //This is our last chance to get the hell out before committing
            if (cancellationToken.IsCancellationRequested)
                return false;

            //Attempt to start the container
            var started = await _dockerClient.Containers.StartContainerAsync(
                createContainerResponse.ID,
                new ContainerStartParameters(),
                new CancellationToken());

            //Check to see if the application started
            if (started)
            {
                //Rename current container to a temporary container name.
                await _dockerClient.Containers.RenameContainerAsync(existingContainer.ID, new ContainerRenameParameters
                {
                    NewName = DockerContainerNames.AgentOutgoing
                }, new CancellationToken());

                //Rename the new container to the production name.
                await _dockerClient.Containers.RenameContainerAsync(createContainerResponse.ID, new ContainerRenameParameters
                {
                    NewName = DockerContainerNames.Agent
                }, new CancellationToken());
                
                //Commit container suicide (this should kill the container that we're in)
                await _dockerClient.Containers.RemoveContainerAsync(existingContainer.ID,
                    new ContainerRemoveParameters()
                    {
                        Force = true
                    }, new CancellationToken());

                //Stop doing update stuff so we don't interfere with the new agent instance.
                await Task.Delay(-1, new CancellationToken());

                //We will never get here
                return true;
            }
                    
            Logger.Warning("Warning: New agent not started. Not entirely sure why.");
            return false;
        }
    }
}