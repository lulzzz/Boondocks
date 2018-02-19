namespace Boondocks.Agent
{
    using System;
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
        private readonly DeviceApiClient _deviceApiClient;
        private readonly ApplicationUpdateService _applicationUpdateService;
        private readonly AgentUpdateService _agentUpdateService;
        private readonly IDockerClient _dockerClient;
        private readonly IDeviceConfiguration _deviceConfiguration;
        private readonly DeviceStateProvider _deviceStateProvider;
        private readonly OperationalStateProvider _operationalStateProvider;
        private readonly ILogger _logger;
        private readonly IUptimeProvider _uptimeProvider;

        public AgentHost(
            IDeviceConfiguration deviceConfiguration,
            IUptimeProvider uptimeProvider,
            DeviceStateProvider deviceStateProvider,
            OperationalStateProvider operationalStateProvider,
            IEnvironmentConfigurationProvider environmentConfigurationProvider, 
            DeviceApiClient deviceApiClient,
            ApplicationUpdateService applicationUpdateService,
            AgentUpdateService agentUpdateService,
            IDockerClient dockerClient,
            ILogger logger)
        {
            _operationalStateProvider = operationalStateProvider ?? throw new ArgumentNullException(nameof(operationalStateProvider));
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
            _applicationUpdateService = applicationUpdateService ?? throw new ArgumentNullException(nameof(applicationUpdateService));
            _agentUpdateService = agentUpdateService ?? throw new ArgumentNullException(nameof(agentUpdateService));
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _logger = logger.ForContext(GetType());
            _deviceStateProvider = deviceStateProvider ?? throw new ArgumentNullException(nameof(deviceStateProvider));
            _uptimeProvider = uptimeProvider ?? throw new ArgumentNullException(nameof(uptimeProvider));
            _deviceConfiguration = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));

            //Config
            _logger.Information("DockerEndpoint: {DockerEndpoint}", environmentConfigurationProvider.DockerEndpoint);
            _logger.Information("DeviceId: {DeviceId}", deviceConfiguration.DeviceId);
            _logger.Information("DeviceApiUrl: {DeviceApiUrl}", deviceConfiguration.DeviceApiUrl);

            //Application version information
            _logger.Information("CurrentApplicationVersion: {CurrentApplicationVersion}", operationalStateProvider.State.CurrentApplicationVersion?.ImageId);
            _logger.Information("NextApplicationVersion: {NextApplicationVersion}", operationalStateProvider.State.NextApplicationVersion?.ImageId);

            _logger.Information("CurrentAgentVersion: {CurrentAgentVersion}", operationalStateProvider.State.CurrentAgentVersion?.ImageId);
            _logger.Information("NextAgentVersion: {NextAgentVersion}", operationalStateProvider.State.NextAgentVersion?.ImageId);
        }

        private async Task LogImagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine("Existing images:");
                Console.WriteLine("---------------------------------------------------------");

                var images = await _dockerClient.Images.ListImagesAsync(new ImagesListParameters()
                {
                    All = true
                }, cancellationToken);

                foreach (var image in images)
                {
                    string tags = string.Join(",", image.RepoTags);

                    Console.WriteLine($"{image.ID} {tags} {image.Created}");
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
                    bool shouldExit = await HeartbeatAsync(cancellationToken);

                    if (shouldExit)
                    {
                        _logger.Information("Exiting RunAsync.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Heartbeat error: {Error}", ex.Message);
                }

                //Wait for a bit.
                await Task.Delay(pollTime, cancellationToken);
            }
        }

        private async Task<bool> HeartbeatAsync(CancellationToken cancellationToken)
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

            //Work on getting the next supervisor.
            if (await _agentUpdateService.UpdateAsync(cancellationToken))
                return true;

            //Do this in case we have a "next" application to download / install
            if (await _applicationUpdateService.UpdateAsync(cancellationToken))
                return true;

            return false;
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

                //Application version
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

                //Configuration version
                if (configuration.SupervisorVersion == null)
                {
                    _logger.Warning("No supervisor version was specified for this device.");
                }
                else
                {
                    if (Equals(_operationalStateProvider.State.CurrentAgentVersion,
                        configuration.SupervisorVersion))
                    {
                        _logger.Verbose("The supervisor version has stayed the same: {ImageId}", configuration.SupervisorVersion?.ImageId);
                    }
                    else
                    {
                        _logger.Information("The supervisor version is now {ImageId}", configuration.SupervisorVersion.ImageId);
                        _operationalStateProvider.State.NextAgentVersion = configuration.SupervisorVersion;
                    }
                }

                _operationalStateProvider.Save();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error getting the current configuration: {ex}", ex.ToString());
            }
        }
    }
}