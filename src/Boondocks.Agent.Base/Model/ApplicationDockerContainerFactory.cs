﻿namespace Boondocks.Agent.Base.Model
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Services.Device.Contracts;

    /// <summary>
    /// Responsible for creating a docker container for an application.
    /// </summary>
    public class ApplicationDockerContainerFactory
    {
        public async Task<CreateContainerResponse> CreateContainerAsync(
            IDockerClient dockerClient, 
            string imageId, 
            EnvironmentVariable[] environmentVariables, 
            CancellationToken cancellationToken)
        {
            string[] formattedEnvironmentVariables = environmentVariables
                .FormatForDevice();

            var createContainerParameters = new CreateContainerParameters()
            {
                Image = imageId,
                HostConfig = new HostConfig()
                {
                    Privileged = true,
                    Mounts = new List<Mount>()
                    {
                        new Mount()
                        {
                            Type = "bind",
                            Source = "/",
                            Target = "/mnt/root"
                        }    
                    },
                    Binds = new List<string>()
                    {
                        
                    },

                    RestartPolicy = new RestartPolicy
                    {
                        Name = RestartPolicyKind.Always
                    },
                },
                Name = DockerContainerNames.Application,
                Env = formattedEnvironmentVariables,
            };

            //Create the container
            return await dockerClient.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);
        }
    }
}