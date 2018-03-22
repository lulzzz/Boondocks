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
    using Services.Device.WebApiClient;

    public class AgentUpdateService : UpdateService
    {
        private readonly IDockerClient _dockerClient;
        private readonly AgentDockerContainerFactory _dockerContainerFactory;
        private readonly IPlatformDetector _platformDetecter;
        private readonly DeviceApiClient _deviceApiClient;

        private const int StartupLoopLimit = 20;

        public AgentUpdateService(
            IDockerClient dockerClient,
            AgentDockerContainerFactory dockerContainerFactory,
            IPlatformDetector platformDetecter,
            DeviceApiClient deviceApiClient,
            ILogger logger) : base(logger, dockerClient)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _dockerContainerFactory = dockerContainerFactory ?? throw new ArgumentNullException(nameof(dockerContainerFactory));
            _platformDetecter = platformDetecter;
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
        }

        public async Task<string> GetCurrentVersionAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //Get all of the containers
            var containers = await _dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    All = true
                }, cancellationToken);

            var a = containers.FirstOrDefault(c => c.Names.Any(n => n.EndsWith(DockerContainerNames.AgentA)));
            var b = containers.FirstOrDefault(c => c.Names.Any(n => n.EndsWith(DockerContainerNames.AgentB)));

            if (a != null)
            {
                return a.ImageID;
            }

            if (b != null)
            {
                return b.ImageID;
            }

            Logger.Fatal("This is bad. I can't find the running agent container, so I can't determine what version it is!");

            return null;
        }

        public async Task StartupAsync(CancellationToken cancellationToken)
        {
            bool done = false;
            int count = 0;

            while (!done)
            {
                try
                {
                    if (count >= StartupLoopLimit)
                    {
                        Logger.Error("Startup loop has exceeded {StartupLoopLimit}. Stopping cycle.", StartupLoopLimit);
                        done = true;
                    }
                    else
                    {
                        //Get all of the containers
                        var containers = await _dockerClient.Containers.ListContainersAsync(
                            new ContainersListParameters()
                            {
                                All = true
                            }, cancellationToken);

                        var a = containers.FirstOrDefault(c => c.Names.Any(n => n.EndsWith(DockerContainerNames.AgentA)));
                        var b = containers.FirstOrDefault(c => c.Names.Any(n => n.EndsWith(DockerContainerNames.AgentB)));

                        int numberOfAgents = 0;

                        if (a != null)
                            numberOfAgents++;

                        if (b != null)
                            numberOfAgents++;

                        if (numberOfAgents == 0)
                        {
                            Logger.Fatal("No agent containers were found. This shouldn't happen.");
                            done = true;
                        }
                        else if (numberOfAgents == 1)
                        {
                            if (a != null)
                            {
                                Logger.Information("Starting up using container 'A'.");
                            }
                            else
                            {
                                Logger.Information("Starting up using container 'B'.");
                            }

                            done = true;
                        }
                        else
                        {
                            //Hmm - it's not safe to start up
                            Logger.Warning(
                                "There is more than one application running. Waiting for the other one to exit.");

                            //Wait a while
                            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warning(ex, "Problem in AgentUpdateService.StartupAsync: {Message}", ex.Message);
                }
                finally
                {
                    count++;
                }
            }
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


            //Get all of the containers
            var containers = await _dockerClient.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    All = true
                }, cancellationToken);

            var a = containers.FirstOrDefault(c => c.Names.Any(n => n.EndsWith(DockerContainerNames.AgentA)));
            var b = containers.FirstOrDefault(c => c.Names.Any(n => n.EndsWith(DockerContainerNames.AgentB)));

            if (a == null && b == null)
            {
                Logger.Fatal("Unable to find the agent container.");
                return false;
            }

            if (a != null && b != null)
            {
                Logger.Fatal("Two agent containers found. Unable to continue update.");
                return false;
            }

            string existingContainerName;
            string newContainerName;

            ContainerListResponse existingContainer;

            if (a != null)
            {
                existingContainerName = DockerContainerNames.AgentA;
                newContainerName = DockerContainerNames.AgentB;
                existingContainer = a;
            }
            else
            {
                existingContainerName = DockerContainerNames.AgentB;
                newContainerName = DockerContainerNames.AgentA;
                existingContainer = b;
            }

            Logger.Information("The existing agent container is {ExistingContainer}. The new one will be {NewContainer}.", existingContainerName, newContainerName);

            //Make sure that the image is downloaded
            if (! await _dockerClient.DoesImageExistAsync(nextVersion.ImageId, cancellationToken))
            {
                await DownloadImageAsync(_dockerClient, nextVersion, cancellationToken);
            }

            Logger.Information("Creating new agent container...");

            //Create the new updated container
            var createContainerResponse = await _dockerContainerFactory.CreateContainerForUpdateAsync(
                _dockerClient,
                nextVersion.ImageId,
                existingContainerName,
                newContainerName,
                cancellationToken);

            //Show the warnings
            if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
            {
                string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                Logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
            }

            Logger.Information("Container {ContainerId} created for agent {ImageId}. Starting new agent...", createContainerResponse.ID, nextVersion.ImageId);

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
                //Commit container suicide (this should kill the container that we're in)
                await _dockerClient.Containers.RemoveContainerAsync(existingContainer.ID,
                    new ContainerRemoveParameters()
                    {
                        Force = true
                    }, new CancellationToken());

                //We may never get here
                return true;
            }
                    
            Logger.Warning("Warning: New agent not started. Not entirely sure why.");
            return false;
        }

        public async Task DownloadImageAsync(IDockerClient dockerClient, VersionReference versionReference,
            CancellationToken cancellationToken)
        {
            var versionRequest = new GetImageDownloadInfoRequest()
            {
                Id = versionReference.Id
            };

            Logger.Information("Getting agent download information for version {ImageId}...", versionReference.ImageId);

            //Get the download info
            var downloadInfo =
                await _deviceApiClient.AgentDownloadInfo.GetAgentVersionDownloadInfo(versionRequest,
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

            Logger.Information("Agent image {ImageId} downloaded.", versionReference.ImageId);
        }
    }
}