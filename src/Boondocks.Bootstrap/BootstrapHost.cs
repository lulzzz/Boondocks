namespace Boondocks.Bootstrap
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Agent.Base;
    using Agent.Base.Model;
    using Agent.Base.Update;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;
    using Services.Device.WebApiClient;

    public class BootstrapHost
    {
        private readonly IDockerClient _dockerClient;
        private readonly DeviceApiClient _deviceApiClient;
        private readonly AgentDockerContainerFactory _agentDockerContainerFactory;
        private readonly AgentUpdateService _agentUpdateService;

        private ILogger Logger { get; }

        public BootstrapHost(
            IDockerClient dockerClient,
            DeviceApiClient deviceApiClient,
            AgentDockerContainerFactory agentDockerContainerFactory,
            AgentUpdateService agentUpdateService,
            ILogger logger)
        {
            _dockerClient = dockerClient;
            _deviceApiClient = deviceApiClient;
            _agentDockerContainerFactory = agentDockerContainerFactory;
            _agentUpdateService = agentUpdateService;
            Logger = logger.ForContext(GetType());
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            bool done = false;

            while (!done)
            {
                try
                {
                    if (await HasRunBefore())
                    {
                        done = true;
                    }
                    else
                    {
                        //Get the version of the agent to install.
                        var deviceConfiguration = await _deviceApiClient.Configuration.GetConfigurationAsync(cancellationToken);

                        //Make sure we have an actual version
                        if (deviceConfiguration.AgentVersion == null)
                        {
                            Logger.Warning("The server isn't giving us a agent version. Can't proceed until that changes.");

                            //Wait a bit before we start over
                            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                        }
                        else
                        {
                            //Make sure that the image is downloaded
                            if (! await _dockerClient.DoesImageExistAsync(deviceConfiguration.AgentVersion.ImageId, cancellationToken))
                            {
                                //Download the image
                                await _agentUpdateService.DownloadImageAsync(_dockerClient, deviceConfiguration.AgentVersion, cancellationToken);
                            }

                            //Create the new updated container
                            var createContainerResponse = await _agentDockerContainerFactory.CreateContainerForDirectAsync(
                                _dockerClient,
                                deviceConfiguration.AgentVersion.ImageId,
                                cancellationToken);

                            //Show the warnings
                            if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
                            {
                                string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                                Logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
                            }

                            Logger.Information("Container {ContainerId} created for agent {ImageId}. Starting new agent...", createContainerResponse.ID, deviceConfiguration.AgentVersion.ImageId);

                            //Attempt to start the container
                            var started = await _dockerClient.Containers.StartContainerAsync(
                                createContainerResponse.ID,
                                new ContainerStartParameters(),
                                cancellationToken);

                            if (!started)
                            {
                                Logger.Fatal("Agent container didn't start. Not sure how to deal with this.");
                            }

                            //We're good! Like totally bootstrapped and stuff.
                            done = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error attempting to bootstrap the agent.");

                    //Wait a bit before we start over
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }

            Logger.Information("We're bootstrapped! Now just wait forever.");

            //Wait forever
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task<bool> HasRunBefore()
        {
            var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true
            });

            string[] names = 
            {
                DockerContainerNames.AgentA,
                DockerContainerNames.AgentB,
                DockerContainerNames.Application
            };

            foreach (var container in containers)
            foreach (var name in names)
            {
                if (container.Names.Any(n => n.EndsWith(name)))
                    return true;
            }


            return false;
        }
    }
}