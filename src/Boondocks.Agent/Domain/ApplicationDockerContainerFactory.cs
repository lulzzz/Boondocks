namespace Boondocks.Agent.Domain
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Shared;

    /// <summary>
    /// Responsible for creating a docker container for an application.
    /// </summary>
    public class ApplicationDockerContainerFactory
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
                            Source = "/mnt/boot/appconfig",
                            Target = "/appconfig",
                        },
                    },

                    RestartPolicy = new RestartPolicy
                    {
                        Name = RestartPolicyKind.Always
                    },
                },
                Name = DockerConstants.ApplicationContainerName,
               
            };

            //Create the container
            return await dockerClient.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);
        }
    }
}