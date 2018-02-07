namespace Boondocks.Agent
{
    using System;
    using System.Collections.Generic;
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
        private readonly IUptimeProvider _uptimeProvider;

        public AgentHost(
            IDeviceConfiguration deviceConfiguration,
            IUptimeProvider uptimeProvider,
            DeviceStateProvider deviceStateProvider,
            ApplicationDockerContainerFactory applicationDockerContainerFactory,
            OperationalStateProvider operationalStateProvider)
        {
            _operationalStateProvider = operationalStateProvider ??
                                        throw new ArgumentNullException(nameof(operationalStateProvider));
            _applicationDockerContainerFactory = applicationDockerContainerFactory ??
                                                 throw new ArgumentNullException(
                                                     nameof(applicationDockerContainerFactory));
            _deviceStateProvider = deviceStateProvider ?? throw new ArgumentNullException(nameof(deviceStateProvider));
            _uptimeProvider = uptimeProvider ?? throw new ArgumentNullException(nameof(uptimeProvider));
            _deviceConfiguration = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));

            _deviceApiClient = new DeviceApiClient(
                _deviceConfiguration.DeviceId,
                _deviceConfiguration.DeviceKey,
                _deviceConfiguration.DeviceApiUrl);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
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
                    Console.WriteLine($"Heartbeat error: {e.Message}");
                }

                //Wait for a bit.
                await Task.Delay(pollTime, cancellationToken);
            }
        }

        private DockerClient CreateDockerClient()
        {
            var dockerClientConfiguration =
                new DockerClientConfiguration(new Uri(_deviceConfiguration.DockerEndpoint));

            return dockerClientConfiguration.CreateClient();
        }

        private async Task DownloadApplicationImageAsync(DockerClient dockerClient, VersionReference versionReference,
            CancellationToken cancellationToken)
        {
            //Dowload it!
            Console.WriteLine($"Downloading application '{versionReference.ImageId}'...");

            Console.WriteLine("Well - not really. It's not implemented yet.");

            //var imageCreateParameters = new ImagesCreateParameters
            //{
            //    Repo = "a repo!",
            //    FromImage = versionReference.ImageId
            //};

            //var authConfig = new AuthConfig();

            ////Do the donwload!!!!!
            //await dockerClient.Images.CreateImageAsync(
            //    imageCreateParameters,
            //    authConfig,
            //    new Progress<JSONMessage>(
            //        m => Console.WriteLine($"\tCreateImageProgress: {m.ProgressMessage}")),
            //    cancellationToken);
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
                    var numberOfRunningContainers = await dockerClient.GetNumberOfRunningContainersAsync(
                        _operationalStateProvider.State.CurrentApplicationVersion.ImageId,
                        cancellationToken);

                    //Check to see if it's running.
                    if (numberOfRunningContainers == 0)
                    {
                        //The current applcation isn't running.
                        Console.WriteLine("The current application isn't running.");

                        //Check to see if the application exists
                        if (!await dockerClient.DoesImageExistAsync(
                            _operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken))
                            await DownloadApplicationImageAsync(dockerClient,
                                _operationalStateProvider.State.CurrentApplicationVersion, cancellationToken);

                        //Try to find the container for this image
                        var container = await dockerClient.GetContainerByImageId(
                            _operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);

                        //Check to see if we found it
                        if (container == null)
                        {
                            //Create the container
                            var createContainerResponse
                                = await _applicationDockerContainerFactory.CreateApplicationContainerAsync(
                                    dockerClient,
                                    _operationalStateProvider.State.CurrentApplicationVersion.ImageId,
                                    cancellationToken);

                            Console.WriteLine($"Container '{createContainerResponse.ID}' created for application.");

                            //Get the container
                            container = await dockerClient.GetContainerByImageId(
                                _operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);
                        }

                        //This shouldn't happen, but we're paranoid.
                        if (container == null)
                        {
                            Console.WriteLine("Unable to find container for application.");
                        }
                        else
                        {
                            //Attempt to start the container
                            var started = await dockerClient.Containers.StartContainerAsync(
                                container.ID,
                                new ContainerStartParameters(),
                                cancellationToken);

                            if (started)
                                Console.WriteLine(
                                    $"Application {_operationalStateProvider.State.CurrentApplicationVersion.Id} started.");
                            else
                                Console.WriteLine("Warning: Application not started.");
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
                    Console.WriteLine("No application version id was specified for this device.");
                }
                else
                {
                    //Check to see if we're supposed to update the application
                    if (!Equals(_operationalStateProvider.State.CurrentApplicationVersion,
                        configuration.ApplicationVersion))
                        _operationalStateProvider.State.NextApplicationVersion = configuration.ApplicationVersion;
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
                    if (!await dockerClient.DoesImageExistAsync(
                        _operationalStateProvider.State.NextApplicationVersion.ImageId, cancellationToken))
                        await DownloadApplicationImageAsync(dockerClient,
                            _operationalStateProvider.State.NextApplicationVersion, cancellationToken);

                    //Stop the existing application
                    if (_operationalStateProvider.State.CurrentApplicationVersion != null)
                        await dockerClient.StopContainersByImageId(
                            _operationalStateProvider.State.CurrentApplicationVersion.ImageId, cancellationToken);

                    //Save this in case we need to delete this version.
                    if (_operationalStateProvider.State.PreviousApplicationVersion != null)
                        _operationalStateProvider.State.ApplicationsToRemove.Add(_operationalStateProvider.State
                            .PreviousApplicationVersion);

                    //Update the operational state
                    _operationalStateProvider.State.CurrentApplicationVersion =
                        _operationalStateProvider.State.NextApplicationVersion;
                    _operationalStateProvider.State.NextApplicationVersion = null;
                    _operationalStateProvider.Save();

                    //Start the new application
                    await EnsureCurrentApplicationRunning(cancellationToken);

                    //TODO: Clean up the old applications
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A problem occurred getting the next application version: '{ex.Message}'");
            }
        }

        /// <summary>
        ///     Gets image ids that are referenced by current / next / previous applications
        /// </summary>
        /// <returns></returns>
        private HashSet<string> GetUsedApplicationImageIds()
        {
            var images = new HashSet<string>();

            images.TryAdd(_operationalStateProvider.State.CurrentApplicationVersion);
            images.TryAdd(_operationalStateProvider.State.PreviousApplicationVersion);
            images.TryAdd(_operationalStateProvider.State.NextApplicationVersion);

            return images;
        }

        /// <summary>
        ///     Delete previous applications.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task GarbageCollectApplications(CancellationToken cancellationToken)
        {
            //Get the applications to remove
            var applicationsToRemove = _operationalStateProvider.State.ApplicationsToRemove?.ToArray();

            if (applicationsToRemove == null || applicationsToRemove.Length == 0)
                return;

            //Create the docker client
            var dockerClient = CreateDockerClient();

            //Remove them!
            foreach (var applicationToRemove in applicationsToRemove)
            {
                //Get the image ids that are in use.
                var usedImageIds = GetUsedApplicationImageIds();

                //Check to see if they're in use.
                if (usedImageIds.Contains(applicationToRemove.ImageId))
                {
                    //Nope. Can't delete it.
                    Console.WriteLine($"Unable to remove image with id '{usedImageIds}'. It is in use.");
                }
                else
                {
                    //Delete the container(s)
                    await dockerClient.RemoveContainersByImageIdAsync(applicationToRemove.ImageId, cancellationToken);

                    //Delete the image
                    await dockerClient.DeleteImageByImageId(applicationToRemove.ImageId, cancellationToken);

                    //Remove this from the list
                    _operationalStateProvider.State.ApplicationsToRemove.Remove(applicationToRemove);
                    _operationalStateProvider.Save();
                }
            }
        }

        private async Task HeartbeatAsync(CancellationToken cancellationToken)
        {
            //Make sure that the current application is running.
            await EnsureCurrentApplicationRunning(cancellationToken);

            //Create the request.
            var request = new HeartbeatRequest
            {
                UptimeSeconds = _uptimeProvider.Ellapsed.TotalSeconds,
                State = _deviceStateProvider.State
            };

            //Send the request.
            var response = await _deviceApiClient.Heartbeat.HeartbeatAsync(request, cancellationToken);

            //Check to see if we need to update the configuration
            if (_operationalStateProvider.State.ConfigurationVersion != response.ConfigurationVersion)
                await DownloadAndUpdateConfigurationAsync(cancellationToken);

            //Clean up the old applications.
            try
            {
                await GarbageCollectApplications(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Problem garbage collecting applications: {ex.Message}");
            }
        }
    }
}