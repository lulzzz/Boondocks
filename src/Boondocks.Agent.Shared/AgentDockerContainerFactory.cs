namespace Boondocks.Agent.Shared
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    public class AgentDockerContainerFactory
    {
        public async Task<CreateContainerResponse> CreateContainerAsync(IDockerClient dockerClient, string imageId, CancellationToken cancellationToken)
        {
            var createContainerParameters = new CreateContainerParameters()
            {
                Image = imageId,
                HostConfig = new HostConfig()
                {
                    Mounts = new List<Mount>()
                                    {
                                        new Mount()
                                        {
                                            Type = "bind",
                                            Source = "/mnt/boot/device.config",
                                            Target = "/mnt/boot/device.config",
                                        }
                                    },
                    Binds = new List<string>()
                                    {
                                        "/var/run/balena.sock:/var/run/balena.sock",
                                    },
                    RestartPolicy = new RestartPolicy
                    {
                        Name = RestartPolicyKind.Always
                    },
                },
                Name = DockerConstants.AgentContainerName,
                Env = new List<string>()
                                {
                                    "DOCKER_ENDPOINT=unix://var/run/balena.sock"
                                },
                Volumes = new Dictionary<string, EmptyStruct>()
                                {
                                    {"/data", new EmptyStruct()},
                                },
            };

            //Create the container
            return await dockerClient.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);
        }
    }
}