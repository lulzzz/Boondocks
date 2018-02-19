namespace Boondocks.Agent.Model
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    internal abstract class DockerContainerFactory
    {
        public async Task<CreateContainerResponse> CreateContainerAsync(IDockerClient dockerClient, string imageId, CancellationToken cancellationToken)
        {
            //Inspect the image to get its configuration
            var inspection = await dockerClient.Images.InspectImageAsync(imageId, cancellationToken);

            var parameters = GetCreationParameters(imageId, inspection.Config);

            //Create the container
            return await dockerClient.Containers.CreateContainerAsync(parameters,
                cancellationToken);
        }

        protected abstract string CreateName();

        protected virtual CreateContainerParameters GetCreationParameters(string imageId, Config config)
        {
            var createContainerParameters = new CreateContainerParameters(config)
            {
                Name = CreateName(),
                Image = imageId,
                HostConfig = new HostConfig
                {
                    RestartPolicy = new RestartPolicy
                    {
                        Name = RestartPolicyKind.Always
                    },
                },
            };

            return createContainerParameters;
        }
    }
}