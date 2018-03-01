namespace Boondocks.Agent.Base.Domain
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
        public static readonly IList<string> ReservedEnvironmentVariables;

        static ApplicationDockerContainerFactory()
        {
            //TODO: Add reserved environment variables as we start using them.

            ReservedEnvironmentVariables = new List<string>()
            {
            }.AsReadOnly();
        }

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
                Env = formattedEnvironmentVariables,
            };

            //Create the container
            return await dockerClient.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);
        }
    }
}