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
        private readonly DeviceStateProvider _deviceStateProvider;
        private readonly UptimeProvider _uptimeProvider;
        private readonly DeviceConfiguration _deviceConfiguration;
        private readonly DeviceApiClient _deviceApiClient;

        public SupervisorHost(
            DeviceConfiguration deviceConfiguration,
            UptimeProvider uptimeProvider,
            DeviceStateProvider _deviceStateProvider)
        {
            this._deviceStateProvider = _deviceStateProvider ?? throw new ArgumentNullException(nameof(_deviceStateProvider));
            _uptimeProvider = uptimeProvider ?? throw new ArgumentNullException(nameof(uptimeProvider));
            _deviceConfiguration = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));

            _deviceApiClient = new DeviceApiClient(
                _deviceConfiguration.DeviceId,
                _deviceConfiguration.DeviceKey,
                _deviceConfiguration.DeviceApiUrl);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await EnsureApplicationRunning(cancellationToken);

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

        private async Task EnsureApplicationRunning(CancellationToken cancellationToken)
        {
            try
            {
                //Get the configuration from the device api
                var configuration = await _deviceApiClient.GetConfigurationAsync(cancellationToken);

                if (configuration.ApplicationVersionId == null)
                {
                    Console.WriteLine("No application version id was specified for this device.");
                }
                else
                {
                    Console.WriteLine(
                        $"Clearing out existing containers and images to load {configuration.ApplicationVersionId}...");

                    var dockerClientConfiguration =
                        new DockerClientConfiguration(new Uri(_deviceConfiguration.DockerEndpoint));

                    var dockerClient = dockerClientConfiguration.CreateClient();

                    //Ditch all of the containers
                    await dockerClient.RemoveAllContainersAsync();

                    //ditch all of the images
                    await dockerClient.DeleteAllImagesAsync();

                    //Download the application image
                    using (var sourceStream =
                        await _deviceApiClient.DownloadApplicationVersionImage(configuration.ApplicationVersionId.Value,
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

                    //Get the images
                    var images = await dockerClient.Images.ListImagesAsync(new ImagesListParameters() {All = true},
                        cancellationToken);

                    //Get the first image
                    var image = images.FirstOrDefault();

                    //Make sure we got it
                    if (image == null)
                    {
                        Console.WriteLine("Unable to find an image");
                    }
                    else
                    {
                        Console.WriteLine($"Creating container for image {image.ID}");

                        var config = new Config()
                        {
                            Hostname = "boondocksapp",
                            Domainname = "",
                            User = "",
                            AttachStdout = true,
                            AttachStderr = true,
                            Env = new string[]
                            {
                                "PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin",
                                "LC_ALL=C.UTF-8",
                                "DEBIAN_FRONTEND=noninteractive",
                                "TINI_VERSION=0.14.0",
                                "container=docker",
                                "BOONDOCKS_VERSION=1.0.0"
                            },
                            Image = image.ID,
                            Volumes = new Dictionary<string, EmptyStruct>()
                            {
                                {"/sys/fs/cgroup", new EmptyStruct()},
                                {"/data", new EmptyStruct()}
                            },
                            WorkingDir = "",
                            Entrypoint = new string[]
                            {
                                "dotnet",
                                "/opt/scada/CaptiveAire.Scada.Module.SystemRunnerHost.dll"
                            },
                            Labels = new Dictionary<string, string>()
                            {
                                {"io.resin.architecture", "armv7hf"},
                                {"io.resin.device-type", "raspberry-pi2"},
                                {"io.resin.qemu.version", "2.9.0.resin1-arm"}
                            },
                            StopSignal = "37",
                        };

                        var createContainerParameters = new CreateContainerParameters(config)
                        {
                            HostConfig = new HostConfig()
                            {
                                ContainerIDFile = "",
                                LogConfig = new LogConfig()
                                {
                                    Type = "journald",
                                    Config = new Dictionary<string, string>(),
                                },
                                NetworkMode = "default",
                                PortBindings = new Dictionary<string, IList<PortBinding>>(),
                                RestartPolicy = new RestartPolicy()
                                {
                                    Name = RestartPolicyKind.No,
                                },
                                VolumeDriver = "",
                                DNS = new List<string>(),
                                DNSOptions = new List<string>(),
                                DNSSearch = new List<string>(),
                                IpcMode = "",
                                PidMode = "",
                                Privileged = true,
                                UTSMode = "",
                                ShmSize = 67108864,
                                ConsoleSize = new ulong[]
                                {
                                    0,
                                    0
                                },
                                Isolation = "",
                                CgroupParent = "",
                                CpusetCpus = "",
                                CpusetMems = "",
                                Devices = new List<DeviceMapping>(),
                                MemorySwappiness = -1,
                                OomKillDisable = false,
                            },

                            NetworkingConfig = new NetworkingConfig()
                            {
                                EndpointsConfig = new Dictionary<string, EndpointSettings>()
                                {
                                    {
                                        "bridge", new EndpointSettings()
                                        {
                                            NetworkID =
                                                "3a0d55dcb98e81107e41061f5e1b769a61eb123791f2ff3fbd270c37a65f1dd6",
                                            EndpointID =
                                                "ca8f87dc4e5ad44eeacdb4178189eeb35cafc3af945394851e8af45aecadc7b7",
                                            Gateway = "172.17.0.1",
                                            IPAddress = "172.17.0.2",
                                            IPPrefixLen = 16,
                                            IPv6Gateway = "",
                                            GlobalIPv6Address = "",
                                            MacAddress = "02:42:ac:11:00:02"
                                        }
                                    }
                                },


                            }
                        };

                        //Create the container
                        var response =
                            await dockerClient.Containers.CreateContainerAsync(createContainerParameters,
                                cancellationToken);

                        Console.WriteLine($"Created container {response.ID}. Starting...");

                        //Star the container.
                        bool started = await dockerClient.Containers.StartContainerAsync(response.ID, new ContainerStartParameters(), cancellationToken);

                        Console.WriteLine($"Container {response.ID} started.");
                    }
                }

                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
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

            Console.WriteLine($"\t{response.ConfigurationVersion}");

            var configuration = await _deviceApiClient.GetConfigurationAsync(cancellationToken);

            foreach (var envVar in configuration.EnvironmentVariables)
            {
                Console.WriteLine($"  {envVar.Name}: {envVar.Value}");
            }

            Console.WriteLine($"Application Version: {configuration.ApplicationVersionId}");
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