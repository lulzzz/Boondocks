namespace Boondocks.Agent
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Interfaces;
    using Model;
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
        private readonly IUptimeProvider _uptimeProvider;

        public AgentHost(
            IDeviceConfiguration deviceConfiguration,
            IUptimeProvider uptimeProvider,
            DeviceStateProvider deviceStateProvider,
            ApplicationDockerContainerFactory applicationDockerContainerFactory,
            OperationalStateProvider operationalStateProvider,
            IEnvironmentConfigurationProvider environmentConfigurationProvider)
        {
            _operationalStateProvider = operationalStateProvider ?? throw new ArgumentNullException(nameof(operationalStateProvider));
            _environmentConfigurationProvider = environmentConfigurationProvider ?? throw new ArgumentNullException(nameof(environmentConfigurationProvider));
            _applicationDockerContainerFactory = applicationDockerContainerFactory ?? throw new ArgumentNullException(nameof(applicationDockerContainerFactory));
            _deviceStateProvider = deviceStateProvider ?? throw new ArgumentNullException(nameof(deviceStateProvider));
            _uptimeProvider = uptimeProvider ?? throw new ArgumentNullException(nameof(uptimeProvider));
            _deviceConfiguration = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));

            //Config
            Console.WriteLine($"DockerEndpoint: {environmentConfigurationProvider.DockerEndpoint}");
            Console.WriteLine($"DeviceId: {deviceConfiguration.DeviceId}");
            Console.WriteLine($"DeviceApiUrl: {deviceConfiguration.DeviceApiUrl}");

            //Application version information
            Console.WriteLine($"CurrentApplicationVersion: {operationalStateProvider.State.CurrentApplicationVersion?.ImageId}");
            Console.WriteLine($"PreviousApplicationVersion: {operationalStateProvider.State.PreviousApplicationVersion?.ImageId}");

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
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
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
                catch (Exception e)
                {
                    Console.WriteLine($"Heartbeat error: {e}");
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

            Console.WriteLine($"Getting application download information for version '{versionReference.ImageId}'...");

            //Get the download info
            var downloadInfo =
                await _deviceApiClient.ApplicationDownloadInfo.GetApplicationVersionDownloadInfo(versionRequest,
                    cancellationToken);

            string fromImage = $"{downloadInfo.Registry}/{downloadInfo.Repository}:{downloadInfo.Name}";

            //Dowload it!
            Console.WriteLine($"Downloading with fromImage = '{fromImage}'...");

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
                new Progress<JSONMessage>(
                    m => Console.WriteLine($"\tCreateImageProgress: {m.ProgressMessage}")),
                cancellationToken);
        }

        //private async Task EnsureCurrentApplicationRunning(CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var dockerClient = CreateDockerClient();

        //        //Check to see if know what version we're supposed to be running.
        //        if (_operationalStateProvider.State.CurrentApplicationVersion == null)
        //        {
        //            Console.WriteLine("No application to run.");
        //        }
        //        else
        //        {
        //            //Get the number of running containers
        //            var numberOfRunningContainers = await dockerClient.GetNumberOfRunningContainersAsync(
        //                _operationalStateProvider.State.CurrentApplicationVersion.ImageId,
        //                cancellationToken);

        //            //Check to see if it's running.
        //            if (numberOfRunningContainers == 0)
        //            {
        //                //The current applcation isn't running.
        //                Console.WriteLine("The current application isn't running.");

        //                //Check to see if the application exists
        //                if (!await dockerClient.DoesImageExistAsync(_operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken))
        //                {
        //                    Console.WriteLine("The image doesn't exist, so download it.");

        //                    await DownloadApplicationImageAsync(dockerClient, _operationalStateProvider.State.CurrentApplicationVersion, cancellationToken);
        //                }

        //                //Try to find the container for this image
        //                var container = await dockerClient.GetContainerByImageId(
        //                    _operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);

        //                //Check to see if we found it
        //                if (container == null)
        //                {
        //                    Console.WriteLine("The container doesn't exist, so create it.");

        //                    //Create the container
        //                    var createContainerResponse
        //                        = await _applicationDockerContainerFactory.CreateApplicationContainerAsync(
        //                            dockerClient,
        //                            _operationalStateProvider.State.CurrentApplicationVersion.ImageId,
        //                            cancellationToken);

        //                    Console.WriteLine($"Container '{createContainerResponse.ID}' created for application.");

        //                    //Get the container
        //                    container = await dockerClient.GetContainerByImageId(
        //                        _operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);
        //                }

        //                //This shouldn't happen, but we're paranoid.
        //                if (container == null)
        //                {
        //                    Console.WriteLine("Unable to find container for application. That's really odd.");
        //                }
        //                else
        //                {

        //                    Console.WriteLine("Staring container...");

        //                    //Attempt to start the container
        //                    var started = await dockerClient.Containers.StartContainerAsync(
        //                        container.ID,
        //                        new ContainerStartParameters(),
        //                        cancellationToken);

        //                    if (started)
        //                    {
        //                        Console.WriteLine($"Application {_operationalStateProvider.State.CurrentApplicationVersion.Id} started.");
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine("Warning: Application not started.");
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("The application container is already running.");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.Error.WriteLine(e.ToString());
        //    }
        //}

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
                    Console.Error.WriteLine("No application version id was specified for this device.");
                }
                else
                {
                    //Check to see if we're supposed to update the application
                    if (Equals(_operationalStateProvider.State.CurrentApplicationVersion,
                        configuration.ApplicationVersion))
                    {
                        Console.WriteLine($"The application version has stayed the same: {configuration.ApplicationVersion.ImageId}");
                    }
                    else
                    {
                        Console.WriteLine($"The application version is now '{configuration.ApplicationVersion.ImageId}'");
                        _operationalStateProvider.State.NextApplicationVersion = configuration.ApplicationVersion;
                    }   
                }

                _operationalStateProvider.Save();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error getting the current configuration: {ex}.");
            }

            try
            {
                //We need to download the next version.
                if (_operationalStateProvider.State.NextApplicationVersion != null)
                {
                    //Create the docker client
                    var dockerClient = CreateDockerClient();

                    //It shouldn't already be downloaded, but check anyway.
                    if (!await dockerClient.DoesImageExistAsync(
                        _operationalStateProvider.State.NextApplicationVersion.ImageId, cancellationToken))
                    {
                        await DownloadApplicationImageAsync(dockerClient, _operationalStateProvider.State.NextApplicationVersion, cancellationToken);
                    }

                    //Stop the existing application
                    if (_operationalStateProvider.State.CurrentApplicationVersion != null)
                    {
                        await dockerClient.StopContainersByImageId(_operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);
                    }

                    //Save this in case we need to delete this version.
                    if (_operationalStateProvider.State.PreviousApplicationVersion != null)
                    {
                        _operationalStateProvider.State.ApplicationsToRemove.Add(_operationalStateProvider.State.PreviousApplicationVersion);
                    }

                    //Update the operational state
                    _operationalStateProvider.State.CurrentApplicationVersion = _operationalStateProvider.State.NextApplicationVersion;
                    _operationalStateProvider.State.NextApplicationVersion = null;
                    _operationalStateProvider.Save();

                    //Download the application
                    await DownloadApplicationImageAsync(dockerClient, _operationalStateProvider.State.CurrentApplicationVersion, cancellationToken);

                    Console.WriteLine($"Create the container for {_operationalStateProvider.State.CurrentApplicationVersion.ImageId} ...");

                    //Create the container
                    var createContainerResponse
                        = await _applicationDockerContainerFactory.CreateApplicationContainerAsync(
                            dockerClient,
                            _operationalStateProvider.State.CurrentApplicationVersion.ImageId,
                            cancellationToken);

                    Console.WriteLine($"Container '{createContainerResponse.ID}' created for application. Starting...");

                    //Attempt to start the container
                    var started = await dockerClient.Containers.StartContainerAsync(
                        createContainerResponse.ID,
                        new ContainerStartParameters(),
                        cancellationToken);

                    if (started)
                    {
                        Console.WriteLine($"Application {_operationalStateProvider.State.CurrentApplicationVersion.ImageId} started.");
                    }
                    else
                    {
                        Console.WriteLine("Warning: Application not started.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"DownloadAndUpdateConfigurationAsync: '{ex}'");
            }
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            ////Make sure that the current application is running.
            //await EnsureCurrentApplicationRunning(cancellationToken);

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
                Console.WriteLine("Heartbeat complete. No new configuration.");
            }
            else
            {

                Console.WriteLine("Downloading new configuration...");
                await DownloadAndUpdateConfigurationAsync(cancellationToken);
            }
        }
    }
}