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
    using Update;

    internal class AgentHost : IAgentHost
    {
        private readonly DeviceApiClient _deviceApiClient;
        private readonly ApplicationUpdateService _applicationUpdateService;
        private readonly AgentUpdateService _agentUpdateService;
        private readonly IDockerClient _dockerClient;
        private readonly IDeviceConfiguration _deviceConfiguration;
        private readonly DeviceStateProvider _deviceStateProvider;
        private readonly ILogger _logger;
        private readonly IUptimeProvider _uptimeProvider;

        private Guid? _configurationVersion;

        public AgentHost(
            IDeviceConfiguration deviceConfiguration,
            IUptimeProvider uptimeProvider,
            DeviceStateProvider deviceStateProvider,
            IEnvironmentConfigurationProvider environmentConfigurationProvider, 
            DeviceApiClient deviceApiClient,
            ApplicationUpdateService applicationUpdateService,
            AgentUpdateService agentUpdateService,
            IDockerClient dockerClient,
            ILogger logger)
        {
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
        }

        private async Task LogStateAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Version information
                _logger.Information("CurrentAgentVersion: {CurrentAgentVersion}", await _agentUpdateService.GetCurrentVersionAsync());
                _logger.Information("CurrentApplicationVersion: {CurrentApplicationVersion}", await _applicationUpdateService.GetCurrentVersionAsync());

                Console.WriteLine();
                Console.WriteLine("Existing images:");
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine();

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
            await LogStateAsync(cancellationToken);

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
                State = _deviceStateProvider.State,
                AgentVersion = await _agentUpdateService.GetCurrentVersionAsync(),
                ApplicationVersion = await _applicationUpdateService.GetCurrentVersionAsync(),
            };

            //Send the request.
            var response = await _deviceApiClient.Heartbeat.HeartbeatAsync(request, cancellationToken);

            //Check to see if we need to update the configuration
            if (_configurationVersion == response.ConfigurationVersion)
            {
                _logger.Verbose("Heartbeat complete. No new configuration.");
            }
            else
            {
                _logger.Information("Downloading new configuration...");
                await DownloadAndProcessConfigurationAsync(cancellationToken);
            }

            //Work on getting the next agent.
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
        private async Task DownloadAndProcessConfigurationAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Get the configuration from the device api
                var configuration = await _deviceApiClient.Configuration.GetConfigurationAsync(cancellationToken);

                //Agent
                await _agentUpdateService.ProcessConfigurationAsync(configuration);

                //Application
                await _applicationUpdateService.ProcessConfigurationAsync(configuration);

                //Save this so we don't keep pinging the server.
                _configurationVersion = configuration.ConfigurationVersion;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error getting the current configuration: {ex}", ex.ToString());
            }
        }
    }
}