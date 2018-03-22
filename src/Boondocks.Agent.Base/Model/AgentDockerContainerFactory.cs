namespace Boondocks.Agent.Base.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    public class AgentDockerContainerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dockerClient"></param>
        /// <param name="imageId">The id of the new agent image.</param>
        /// <param name="existingContainerName">The name of the existing container</param>
        /// <param name="incomingContainerName">The name of the new container to create.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CreateContainerResponse> CreateContainerForUpdateAsync(
            IDockerClient dockerClient, 
            string imageId, 
            string existingContainerName, 
            string incomingContainerName,  
            CancellationToken cancellationToken)
        {
            var existingContainer =
                await dockerClient.GetContainerByName(existingContainerName, cancellationToken);

            if (existingContainer == null)
            {
                string message = $"Unable to find container '{existingContainerName}'";
                Console.Error.WriteLine(message);
                throw new Exception(message);
            }

            //Get the existing container
            var existingContainerInspection = await dockerClient.Containers.InspectContainerAsync(existingContainer.ID, cancellationToken);

            if (existingContainerInspection == null)
            {
                string message = $"Unable to inspect container '{existingContainer.ID}'";
                Console.Error.WriteLine(message);
                throw new Exception(message);
            }

            var createContainerParameters = new CreateContainerParameters
            {
                Image = imageId,
                Name = incomingContainerName,
                HostConfig = existingContainerInspection.HostConfig,
                Env = existingContainerInspection.Config.Env
            };

            //Create it!!!! 
            return await dockerClient.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);
        }

        /// <summary>
        /// This is intended for use in a development scenario. This is so the command line can create a 
        /// </summary>
        /// <param name="dockerClient"></param>
        /// <param name="imageId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CreateContainerResponse> CreateContainerForDirectAsync(IDockerClient dockerClient, string imageId, CancellationToken cancellationToken)
        {
            var createContainerParameters = new CreateContainerParameters
            {
                Image = imageId,
                HostConfig = new HostConfig()
                {
                    Mounts = new List<Mount>()
                                    {
                                        new Mount()
                                        {
                                            Type = "bind",
                                            Source = "/mnt/boot/",
                                            Target = "/mnt/boot/",
                                        },
                                        new Mount()
                                        {
                                            Type="bind",
                                            Source="/mnt/data/resin-data/resin-supervisor",
                                            Target = "/data"
                                        },
                                        new Mount()
                                        {
                                            Type = "bind",
                                            Source="/mnt/boot/config.json",
                                            Target = "/boot/config.json",
                                        },
                                        new Mount()
                                        {
                                            Type = "bind",
                                            Source = "/",
                                            Target = "/mnt/root"
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
                Name = DockerContainerNames.AgentA,
                Env = new List<string>()
                                {
                                    "DOCKER_SOCKET=/var/run/balena.sock",
                                    "DOCKER_ROOT=/mnt/root/var/lib/docker",
                                    "BOOT_MOUNTPOINT=/mnt/boot",
                                    "container=docker"
                                },
                
            };

            //Create the container
            return await dockerClient.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);
        }
    }
}