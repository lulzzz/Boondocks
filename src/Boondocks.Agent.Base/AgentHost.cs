namespace Boondocks.Agent.Base
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Interfaces;
    using Logs;
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
        private readonly IRootFileSystemUpdateService _rootFileSystemUpdateService;
        private readonly AgentUpdateService _agentUpdateService;
        private readonly IDockerClient _dockerClient;
        private readonly ApplicationLogSucker _applicationLogSucker;
        private readonly IDeviceConfiguration _deviceConfiguration;
        private readonly DeviceStateProvider _deviceStateProvider;
        private readonly ILogger _logger;
        private readonly IUptimeProvider _uptimeProvider;

        private Guid? _configurationVersion;

        private readonly UpdateService[] _updateServices;

        public AgentHost(
            IDeviceConfiguration deviceConfiguration,
            IUptimeProvider uptimeProvider,
            DeviceStateProvider deviceStateProvider,
            IPathFactory pathFactory, 
            DeviceApiClient deviceApiClient,
            ApplicationUpdateService applicationUpdateService,
            IRootFileSystemUpdateService rootFileSystemUpdateService,
            AgentUpdateService agentUpdateService,
            IDockerClient dockerClient,
            ApplicationLogSucker applicationLogSucker,
            ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
            _applicationUpdateService = applicationUpdateService ?? throw new ArgumentNullException(nameof(applicationUpdateService));
            _rootFileSystemUpdateService = rootFileSystemUpdateService ?? throw new ArgumentNullException(nameof(rootFileSystemUpdateService));
            _agentUpdateService = agentUpdateService ?? throw new ArgumentNullException(nameof(agentUpdateService));
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _applicationLogSucker = applicationLogSucker ?? throw new ArgumentNullException(nameof(applicationLogSucker));
            _logger = logger.ForContext(GetType());
            _deviceStateProvider = deviceStateProvider ?? throw new ArgumentNullException(nameof(deviceStateProvider));
            _uptimeProvider = uptimeProvider ?? throw new ArgumentNullException(nameof(uptimeProvider));
            _deviceConfiguration = deviceConfiguration;

            //Config
            _logger.Information("Docker Endpoint: {DockerSocket}", pathFactory.DockerEndpoint);
            _logger.Information("DeviceId: {DeviceId}", deviceConfiguration?.DeviceId);
            _logger.Information("DeviceApi Url: {DeviceApiUrl}", deviceConfiguration?.DeviceApiUrl);

            //The agent should be updated before the application
            _updateServices = new UpdateService[]
            {
                _agentUpdateService,
                _applicationUpdateService
            };
        }

        private async Task LogStateAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Version information
                _logger.Information("Agent Version: {CurrentAgentVersion}", await _agentUpdateService.GetCurrentVersionAsync(cancellationToken));
                _logger.Information("Application Version: {CurrentApplicationVersion}", await _applicationUpdateService.GetCurrentVersionAsync(cancellationToken));
                _logger.Information("Root File System Version: {RootFileSystemVersion}", await _rootFileSystemUpdateService.GetCurrentVersionAsync(cancellationToken));

                Console.WriteLine();
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

                Console.WriteLine();

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error logging images: {Error}", ex.ToString());
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await LogStateAsync(cancellationToken);

            await _agentUpdateService.StartupAsync(cancellationToken);

            //Start up the application log sucker
            Task logSuckerTask = _applicationLogSucker.SuckAsync(cancellationToken);

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

        /// <summary>
        /// Periodic communication with the service.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>True if the agent is to exit, false otherwise.</returns>
        private async Task<bool> HeartbeatAsync(CancellationToken cancellationToken)
        {
            //Create the request.
            var request = new HeartbeatRequest
            {
                UptimeSeconds = _uptimeProvider.Ellapsed.TotalSeconds,
                State = _deviceStateProvider.State,
                AgentVersion = await _agentUpdateService.GetCurrentVersionAsync(cancellationToken),
                ApplicationVersion = await _applicationUpdateService.GetCurrentVersionAsync(cancellationToken),
                RootFileSystemVersion = await _rootFileSystemUpdateService.GetCurrentVersionAsync(cancellationToken)
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

                return await DownloadAndProcessConfigurationAsync(cancellationToken);
            }

            return false;
        }

        /// <summary>
        /// Downloads the configuration for this device and saves it.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> DownloadAndProcessConfigurationAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Get the configuration from the device api
                var configuration = await _deviceApiClient.Configuration.GetConfigurationAsync(cancellationToken);

                foreach (var updateService in _updateServices)
                {
                    if (await updateService.UpdateAsync(configuration, cancellationToken))
                        return true;
                }

                //Save this so we don't keep pinging the server.
                _configurationVersion = configuration.ConfigurationVersion;
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Error getting the current configuration: {ex}", ex.ToString());
            }

            return false;
        }
    }
}