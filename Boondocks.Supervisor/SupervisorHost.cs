using System;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApiClient;
using Boondocks.Supervisor.Model;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Boondocks.Supervisor
{
    public class SupervisorHost
    {
        private readonly OperationalStateProvider _operationalStateProvider;
        private readonly ApplicationContainerFactory _applicationContainerFactory;
        private readonly DeviceStateProvider _deviceStateProvider;
        private readonly UptimeProvider _uptimeProvider;
        private readonly DeviceConfiguration _deviceConfiguration;
        private readonly DeviceApiClient _deviceApiClient;

        public SupervisorHost(
            DeviceConfiguration deviceConfiguration,
            UptimeProvider uptimeProvider,
            DeviceStateProvider deviceStateProvider,
            ApplicationContainerFactory applicationContainerFactory,
            OperationalStateProvider operationalStateProvider)
        {
            _operationalStateProvider = operationalStateProvider ?? throw new ArgumentNullException(nameof(operationalStateProvider));
            _applicationContainerFactory = applicationContainerFactory ?? throw new ArgumentNullException(nameof(applicationContainerFactory));
            _deviceStateProvider = deviceStateProvider ?? throw new ArgumentNullException(nameof(deviceStateProvider));
            _uptimeProvider = uptimeProvider ?? throw new ArgumentNullException(nameof(uptimeProvider));
            _deviceConfiguration = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));

            _deviceApiClient = new DeviceApiClient(
                _deviceConfiguration.DeviceId,
                _deviceConfiguration.DeviceKey,
                _deviceConfiguration.DeviceApiUrl);
        }

        private DockerClient CreateDockerClient()
        {
            var dockerClientConfiguration =
                new DockerClientConfiguration(new Uri(_deviceConfiguration.DockerEndpoint));

            return dockerClientConfiguration.CreateClient();
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            //This is how long we'll wait inbetween heartbeats.
            TimeSpan pollTime = TimeSpan.FromSeconds(_deviceConfiguration.PollSeconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await HeartbeatAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                //Wait for a bit.
                await Task.Delay(pollTime, cancellationToken);
            }
        }

        private async Task DownloadApplicationImageAsync(DockerClient dockerClient, Guid id, CancellationToken cancellationToken)
        {
            //Dowload it!
            Console.WriteLine($"Downloading application '{id}'...");

            //Download the application image
            using (var sourceStream =
                await _deviceApiClient.DownloadApplicationVersionImage(id,
                    cancellationToken))
            {
                //Load it up into docker as we download it
                await dockerClient.Images.LoadImageAsync(new ImageLoadParameters()
                    {
                        Quiet = false
                    }, sourceStream,
                    new Progress(),
                    cancellationToken);
            }
        }

        private async Task EnsureCurrentApplicationRunning(CancellationToken cancellationToken)
        {
            try
            {
                var dockerClient = CreateDockerClient();

                //Check to see if know what version we're supposed to be running.
                if (_operationalStateProvider.State.CurrentApplicationVersion == null)
                {
                    Console.WriteLine("No application to run.");
                }
                else
                {
                    //Get the number of running containers
                    int numberOfRunningContainers = await dockerClient.GetNumberOfRunningContainersAsync(
                        _operationalStateProvider.State.CurrentApplicationVersion.ImageId, 
                        cancellationToken);

                    //Check to see if it's running.
                    if (numberOfRunningContainers == 0)
                    {
                        //The current applcation isn't running.
                        Console.WriteLine("The current application isn't running.");

                        //Check to see if the application exists
                        if (!await dockerClient.DoesImageExistAsync(_operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken))
                        {
                            //Download the image
                            await DownloadApplicationImageAsync(dockerClient, _operationalStateProvider.State.CurrentApplicationVersion.Id, cancellationToken);
                        }

                        //Try to find the container for this image
                        var container = await dockerClient.GetContainerByImageId(_operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);

                        //Check to see if we found it
                        if (container == null)
                        {
                            //Create the container
                            CreateContainerResponse createContainerResponse
                                = await _applicationContainerFactory.CreateApplicationContainerAsync(
                                    dockerClient,
                                    _operationalStateProvider.State.CurrentApplicationVersion.ImageId,
                                    cancellationToken);

                            Console.WriteLine($"Container '{createContainerResponse.ID}' created for application.");

                            //Get the container
                            container = await dockerClient.GetContainerByImageId(_operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);
                        }

                        //This shouldn't happen, but we're paranoid.
                        if (container == null)
                        {
                            Console.WriteLine("Unable to find container for application.");
                        }
                        else
                        {
                            //Attempt to start the container
                            bool started = await dockerClient.Containers.StartContainerAsync(
                                container.ID,
                                new ContainerStartParameters(),
                                cancellationToken);

                            if (started)
                            {
                                Console.WriteLine($"Application {_operationalStateProvider.State.CurrentApplicationVersion.Id} started.");
                            }
                            else
                            {
                                Console.WriteLine("Warning: Application not started.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("The application container is already running.");
                    }
                }

            
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }

        /// <summary>
        /// Downloads the configuration for this device and saves it.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task DownloadAndUpdateConfigurationAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Get the configuration from the device api
                var configuration = await _deviceApiClient.GetConfigurationAsync(cancellationToken);

                //Save the configuration version.
                _operationalStateProvider.State.ConfigurationVersion = configuration.ConfigurationVersion;

                if (configuration.ApplicationVersion == null)
                {
                    Console.WriteLine("No application version id was specified for this device.");
                }
                else
                {
                    //Check to see if we're supposed to update the application
                    if (!Equals(_operationalStateProvider.State.CurrentApplicationVersion, configuration.ApplicationVersion))
                    {
                        //This is going to be the next application version
                        _operationalStateProvider.State.NextApplicationVersion = configuration.ApplicationVersion;
                    }
                }

                _operationalStateProvider.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting the current configuration: {ex.Message}.");                
            }

            try
            {
                //We need to download the next version.
                if (_operationalStateProvider.State.NextApplicationVersion != null)
                {
                    //Create the docker client
                    var dockerClient = CreateDockerClient();

                    //It shouldn't already be downloaded, but check anyway.
                    if (!await dockerClient.DoesImageExistAsync(_operationalStateProvider.State.NextApplicationVersion.ImageId, cancellationToken))
                    {
                        //Attempt to download
                        await DownloadApplicationImageAsync(dockerClient, _operationalStateProvider.State.NextApplicationVersion.Id, cancellationToken);
                    }

                    //Stop the existing application
                    if (_operationalStateProvider.State.CurrentApplicationVersion != null)
                    {
                        //Stop the container
                        await dockerClient.StopContainersByImageId(
                            _operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);
                    }

                    //Update the operational state
                    _operationalStateProvider.State.CurrentApplicationVersion = _operationalStateProvider.State.NextApplicationVersion;
                    _operationalStateProvider.State.NextApplicationVersion = null;
                    _operationalStateProvider.Save();

                    //Start the new application
                    await EnsureCurrentApplicationRunning(cancellationToken);

                    //TODO: Clean up the old application
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A problem occurred getting the next application version: '{ex.Message}'");
            }
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            //Make sure that the current application is running.
            await EnsureCurrentApplicationRunning(cancellationToken);

            //Create the request.
            var request = new HeartbeatRequest()
            {
                UptimeSeconds = _uptimeProvider.Ellapsed.TotalSeconds,
                State = _deviceStateProvider.State
            };

            //Send the request.
            var response = await _deviceApiClient.HeartbeatAsync(request, cancellationToken);

            //Check to see if we need to update the configuration
            if (_operationalStateProvider.State.ConfigurationVersion != response.ConfigurationVersion)
            {
                //Update the configuration
                await DownloadAndUpdateConfigurationAsync(cancellationToken);

                //Save it
                _operationalStateProvider.Save();
            }
        }
    }
}