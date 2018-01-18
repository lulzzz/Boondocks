using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Boondocks.Agent.Model
{
    /// <summary>
    /// Responsible for creating a docker container for an application.
    /// </summary>
    internal class ApplicationDockerContainerFactory
    {
        public Task<CreateContainerResponse> CreateApplicationContainerAsync(DockerClient dockerClient, string imageId, CancellationToken cancellationToken)
        {
            CreateContainerParameters parameters = GetCreationParameters(imageId);

            //Create the container
            return dockerClient.Containers.CreateContainerAsync(parameters,
                    cancellationToken);
        }

        private CreateContainerParameters GetCreationParameters(string imageId)
        {
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
                Image = imageId,
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
                                            NetworkID = "3a0d55dcb98e81107e41061f5e1b769a61eb123791f2ff3fbd270c37a65f1dd6",
                                            EndpointID = "ca8f87dc4e5ad44eeacdb4178189eeb35cafc3af945394851e8af45aecadc7b7",
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

            return createContainerParameters;
        }
    }
}