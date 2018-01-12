using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            //Startup
            await EnsureApplicationRunning(cancellationToken);

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
                //Load it up
                await dockerClient.Images.LoadImageAsync(new ImageLoadParameters()
                    {
                        Quiet = false
                    }, sourceStream,
                    new Progress(),
                    cancellationToken);
            }
        }

        private async Task EnsureApplicationRunning(CancellationToken cancellationToken)
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
                        Console.WriteLine("The curreng application isn't running.");

                        //Check to see if the application exists
                        if (!await dockerClient.DoesImageExistAsync(_operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken))
                        {
                            //Download the image
                            await DownloadApplicationImageAsync(dockerClient, _operationalStateProvider.State.CurrentApplicationVersion.ApplicationVersionId, cancellationToken);
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

                            //Get the container
                            container = await dockerClient.GetContainerByImageId(_operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);
                        }

                        bool started = await dockerClient.Containers.StartContainerAsync(
                            container.ID, 
                            new ContainerStartParameters(),
                            cancellationToken);

                        if (started)
                        {
                            Console.WriteLine($"Application {_operationalStateProvider.State.CurrentApplicationVersion.ApplicationVersionId} started.");
                        }
                        else
                        {
                            Console.WriteLine("Warning: Application not started.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("The application container is already running.");
                    }
                }

                ////Get the configuration from the device api
                //var configuration = await _deviceApiClient.GetConfigurationAsync(cancellationToken);

                //if (configuration.ApplicationVersion == null)
                //{
                //    Console.WriteLine("No application version id was specified for this device.");
                //}
                //else
                //{
                //    Console.WriteLine($"Clearing out existing containers and images to load {configuration.ApplicationVersion}...");

                    
                //    //Ditch all of the containers
                //    await dockerClient.RemoveAllContainersAsync();

                //    //ditch all of the images
                //    await dockerClient.DeleteAllImagesAsync();

                //    if (configuration.ApplicationVersion == null)
                //    {
                //        Console.WriteLine("No application.");
                //        return;
                //    }


                //    //Get the images
                //    var images = await dockerClient.Images.ListImagesAsync(new ImagesListParameters() {All = true},
                //        cancellationToken);

                //    //Get the first image
                //    var image = images.FirstOrDefault(i => i.ID == configuration.ApplicationVersion.ImageId);

                //    //Make sure we got it
                //    if (image == null)
                //    {
                //        Console.WriteLine($"Unable to find image '{configuration.ApplicationVersion.ImageId}'");
                //    }
                //    else
                //    {
                //        Console.WriteLine($"Creating container for image {image.ID}");

                //        //Create the container
                //        var response =
                //            await _applicationContainerFactory.CreateApplicationContainerAsync(dockerClient, image,
                //                cancellationToken);

                //        Console.WriteLine($"Created container {response.ID}. Starting...");

                //        //Star the container.
                //        bool started = await dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters(), cancellationToken);

                //        Console.WriteLine($"Container {response.ID} started: {started}.");
                //    }
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }

        private async Task UpdateConfigurationAsync(CancellationToken cancellationToken)
        {
            var dockerClient = CreateDockerClient();

            //Get the configuration from the device api
            var configuration = await _deviceApiClient.GetConfigurationAsync(cancellationToken);

            if (configuration.ApplicationVersion == null)
            {
                Console.WriteLine("No application version id was specified for this device.");

                _operationalStateProvider.State.CurrentApplicationVersion = null;
            }
            else
            {
                Console.WriteLine($"Clearing out existing containers and images to load {configuration.ApplicationVersion}...");

                //Ditch all of the containers
                await dockerClient.RemoveAllContainersAsync();

                //ditch all of the images
                await dockerClient.DeleteAllImagesAsync();

                if (configuration.ApplicationVersion == null)
                {
                    Console.WriteLine("No application.");
                    return;
                }

            }

            _operationalStateProvider.Save();
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
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
                await UpdateConfigurationAsync(cancellationToken);

                //Store the new configuration version
                _operationalStateProvider.State.ConfigurationVersion = response.ConfigurationVersion;

                //Save it
                _operationalStateProvider.Save();
            }
        }
    }

    public class Progress :  IProgress<JSONMessage>
    {
        public void Report(JSONMessage value)
        {
            Console.WriteLine($"    {value.ProgressMessage}");
        }
    }
}