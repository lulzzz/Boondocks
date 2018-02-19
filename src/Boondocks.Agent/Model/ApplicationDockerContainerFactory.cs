namespace Boondocks.Agent.Model
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;

    /// <summary>
    /// Responsible for creating a docker container for an application.
    /// </summary>
    internal class ApplicationDockerContainerFactory : DockerContainerFactory
    {
        public ApplicationDockerContainerFactory(ILogger logger) : base(logger)
        {

        }

        public override async Task<CreateContainerResponse> CreateContainerAsync(IDockerClient dockerClient, string imageId, CancellationToken cancellationToken)
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

            Logger.Information("Creating application docker container from image {ImageId}...", imageId);

            //Create the container
            return await dockerClient.Containers.CreateContainerAsync(createContainerParameters, cancellationToken);
        }
    }
}