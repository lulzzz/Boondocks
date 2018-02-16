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
    using Services.Contracts.Interfaces;
    using Services.Device.Contracts;
    using Services.Device.WebApiClient;

    internal class AgentHost : IAgentHost
    {
        private readonly ApplicationDockerContainerFactory _applicationDockerContainerFactory;
        private readonly DeviceApiClient _deviceApiClient;
        private readonly IDeviceConfiguration _deviceConfiguration;
        private readonly DeviceStateProvider _deviceStateProvider;
        private readonly OperationalStateProvider _operationalStateProvider;
        private readonly IEnvironmentConfigurationProvider _environmentConfigurationProvider;
        private readonly ILogger _logger;
        private readonly IUptimeProvider _uptimeProvider;

        public AgentHost(
            IDeviceConfiguration deviceConfiguration,
            IUptimeProvider uptimeProvider,
            DeviceStateProvider deviceStateProvider,
            ApplicationDockerContainerFactory applicationDockerContainerFactory,
            OperationalStateProvider operationalStateProvider,
            IEnvironmentConfigurationProvider environmentConfigurationProvider, 
            ILogger logger)
        {
            _operationalStateProvider = operationalStateProvider ?? throw new ArgumentNullException(nameof(operationalStateProvider));
            _environmentConfigurationProvider = environmentConfigurationProvider ?? throw new ArgumentNullException(nameof(environmentConfigurationProvider));
            _logger = logger.ForContext(GetType());
            _applicationDockerContainerFactory = applicationDockerContainerFactory ?? throw new ArgumentNullException(nameof(applicationDockerContainerFactory));
            _deviceStateProvider = deviceStateProvider ?? throw new ArgumentNullException(nameof(deviceStateProvider));
            _uptimeProvider = uptimeProvider ?? throw new ArgumentNullException(nameof(uptimeProvider));
            _deviceConfiguration = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));

            //Config
            _logger.Information("DockerEndpoint: {DockerEndpoint}", environmentConfigurationProvider.DockerEndpoint);
            _logger.Information("DeviceId: {DeviceId}", deviceConfiguration.DeviceId);
            _logger.Information("DeviceApiUrl: {DeviceApiUrl}", deviceConfiguration.DeviceApiUrl);

            //Application version information
            _logger.Information("CurrentApplicationVersion: {CurrentApplicationVersion}", operationalStateProvider.State.CurrentApplicationVersion?.ImageId);
            _logger.Information("PreviousApplicationVersion: {PreviousApplicationVersion}", operationalStateProvider.State.PreviousApplicationVersion?.ImageId);

            _deviceApiClient = new DeviceApiClient(
                deviceConfiguration.DeviceId,
                deviceConfiguration.DeviceKey,
                deviceConfiguration.DeviceApiUrl);
        }

        private async Task LogImagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Existing images:");
                Console.WriteLine("---------------------------------------------------------");

                using (var dockerClient = CreateDockerClient())
                {
                    var images = await dockerClient.Images.ListImagesAsync(new ImagesListParameters()
                    {
                        All = true
                    }, cancellationToken);

                    foreach (var image in images)
                    {
                        string tags = string.Join(",", image.RepoTags);

                        Console.WriteLine($"{image.ID} {tags} {image.Created}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error logging images: {Error}", ex.ToString());
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await LogImagesAsync(cancellationToken);

            //This is how long we'll wait inbetween heartbeats.
            var pollTime = TimeSpan.FromSeconds(_deviceConfiguration.PollSeconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await HeartbeatAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Heartbeat error: {Error}", ex.Message);
                }

                //Wait for a bit.
                await Task.Delay(pollTime, cancellationToken);
            }
        }

        private DockerClient CreateDockerClient()
        {
            var dockerClientConfiguration =
                new DockerClientConfiguration(new Uri(_environmentConfigurationProvider.DockerEndpoint));

            return dockerClientConfiguration.CreateClient();
        }

        private async Task DownloadApplicationImageAsync(DockerClient dockerClient, VersionReference versionReference,
            CancellationToken cancellationToken)
        {
            var versionRequest = new GetImageDownloadInfoRequest()
            {
                Id = versionReference.Id
            };

            _logger.Information("Getting application download information for version {ImageId}...", versionReference.ImageId);

            //Get the download info
            var downloadInfo =
                await _deviceApiClient.ApplicationDownloadInfo.GetApplicationVersionDownloadInfo(versionRequest,
                    cancellationToken);

            string fromImage = $"{downloadInfo.Registry}/{downloadInfo.Repository}:{downloadInfo.Name}";

            //Dowlnoad it!
            _logger.Information("Downloading with fromImage = '{FromImage}'...", fromImage);

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

            _logger.Information("Image {ImageId} downloaded.", versionReference.ImageId);
        }

        private async Task NextApplicationVersionProcessAsync(CancellationToken cancellationToken)
        {
            try
            {
                //We need to download the next version.
                if (_operationalStateProvider.State.NextApplicationVersion != null)
                {
                    string imageId = _operationalStateProvider.State.NextApplicationVersion.ImageId;

                    //Create the docker client
                    using (var dockerClient = CreateDockerClient())
                    {
                        //It shouldn't already be downloaded, but check anyway.
                        if (!await dockerClient.DoesImageExistAsync(imageId, cancellationToken))
                        {
                            //Download the application
                            await DownloadApplicationImageAsync(dockerClient, _operationalStateProvider.State.NextApplicationVersion, cancellationToken);
                        }

                        //Ditch the current applications
                        await StopAndDestroyApplicationAsync(dockerClient, cancellationToken);
                        
                        _logger.Information("Create the container for {ImageId} ...", imageId);

                        //Create the container
                        var createContainerResponse = await _applicationDockerContainerFactory.CreateApplicationContainerAsync(
                                dockerClient,
                                imageId,
                                cancellationToken);

                        if (createContainerResponse.Warnings != null && createContainerResponse.Warnings.Any())
                        {
                            string formattedWarnings = string.Join(",", createContainerResponse.Warnings);

                            _logger.Warning("Warnings during container creation: {Warnings}", formattedWarnings);
                        }

                        _logger.Information("Container {ContainerId} created for application {}. Starting...", createContainerResponse.ID, imageId);
                        
                        //Attempt to start the container
                        var started = await dockerClient.Containers.StartContainerAsync(
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
                            _logger.Warning("Warning: Application not started.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warning($"DownloadAndUpdateConfigurationAsync: '{ex}'");
            }
        }

        private async Task StopAndDestroyApplicationAsync(DockerClient dockerClient, CancellationToken cancellationToken)
        {
            //Get all of the containers
            var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true
            }, cancellationToken);

            //Find all of the application containers (should should be one)
            var containersToDelete = containers
                .Where(c => c.Names.Any(n => n.EndsWith(DockerConstants.ApplicationContainerName)))
                .ToArray();

            //Create the parameters
            var parameters = new ContainerRemoveParameters()
            {
                Force = true
            };

            //Delete each one
            foreach (var container in containersToDelete)
            {
                _logger.Information("Removing application container {ContainerId} with image {ImageId}", container.ID, container.ImageID);

                //Delete it
                await dockerClient.Containers.RemoveContainerAsync(container.ID, parameters, cancellationToken);
            }
        }

        /// <summary>
        ///     Downloads the configuration for this device and saves it.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task DownloadAndUpdateConfigurationAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Get the configuration from the device api
                var configuration = await _deviceApiClient.Configuration.GetConfigurationAsync(cancellationToken);

                //Save the configuration version.
                _operationalStateProvider.State.ConfigurationVersion = configuration.ConfigurationVersion;

                if (configuration.ApplicationVersion == null)
                {
                    _logger.Information("No application version id was specified for this device.");
                }
                else
                {
                    //Check to see if we're supposed to update the application
                    if (Equals(_operationalStateProvider.State.CurrentApplicationVersion, configuration.ApplicationVersion))
                    {
                        _logger.Verbose("The application version has stayed the same: {ImageId}", configuration.ApplicationVersion.ImageId);
                    }
                    else
                    {
                        _logger.Information("The application version is now {ImageId}", configuration.ApplicationVersion.ImageId);
                        _operationalStateProvider.State.NextApplicationVersion = configuration.ApplicationVersion;
                    }   
                }

                _operationalStateProvider.Save();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error getting the current configuration: {ex}", ex.ToString());
            }
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            //Create the request.
            var request = new HeartbeatRequest
            {
                UptimeSeconds = _uptimeProvider.Ellapsed.TotalSeconds,
                State = _deviceStateProvider.State
            };

            //Send the request.
            var response = await _deviceApiClient.Heartbeat.HeartbeatAsync(request, cancellationToken);

            //Check to see if we need to update the configuration
            if (_operationalStateProvider.State.ConfigurationVersion == response.ConfigurationVersion)
            {
                _logger.Verbose("Heartbeat complete. No new configuration.");
            }
            else
            {
                _logger.Information("Downloading new configuration...");
                await DownloadAndUpdateConfigurationAsync(cancellationToken);
            }

            //Do this in case we have a "next" application to download / install
            await NextApplicationVersionProcessAsync(cancellationToken);
        }
    }
}