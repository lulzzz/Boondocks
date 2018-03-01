namespace Boondocks.Agent.Base
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    internal static class DockerExtensions
    {
        public static async Task StartAllContainers(this IDockerClient client)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            });

            foreach (var container in containers)
            {
                Console.WriteLine($"Starting container {container.ID}...");

                await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());
            }
        }

        public static async Task RemoveAllContainersAsync(this IDockerClient client)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            });

            foreach (var container in containers)
            {
                Console.WriteLine($"Deleting container {container.ID}...");

                await client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters
                {
                    Force = true
                });
            }
        }

        public static async Task DeleteAllImagesAsync(this IDockerClient client)
        {
            var images = await client.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true
            });

            foreach (var image in images)
            {
                Console.WriteLine($"Deleting image '{image.ID}'...");

                await client.Images.DeleteImageAsync(image.ID, new ImageDeleteParameters
                {
                    Force = true,
                    PruneChildren = true
                });
            }
        }

        public static async Task DeleteImageByImageId(this IDockerClient client, string imageId,
            CancellationToken cancellationToken)
        {
            var images = await client.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true
            }, cancellationToken);

            foreach (var image in images.Where(i => i.ID == imageId))
            {
                Console.WriteLine($"Deleting image '{image.ID}'...");

                await client.Images.DeleteImageAsync(image.ID, new ImageDeleteParameters
                {
                    Force = true,
                    PruneChildren = true
                }, cancellationToken);
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="imageId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The number of containers removed.</returns>
        public static async Task<int> RemoveContainersByImageIdAsync(this IDockerClient client, string imageId,
            CancellationToken cancellationToken)
        {
            var containersRemovedCount = 0;

            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            }, cancellationToken);

            foreach (var container in containers.Where(c => c.ImageID == imageId))
            {
                Console.WriteLine($"Deleting container {container.ID}...");

                await client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters
                {
                    Force = false
                }, cancellationToken);

                containersRemovedCount++;
            }

            return containersRemovedCount;
        }

        /// <summary>
        ///     Should theoretically only ever stop one container.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="imageId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task StopContainersByImageId(this IDockerClient client, string imageId,
            CancellationToken cancellationToken = new CancellationToken())
        {
            //List the running containers
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters(), cancellationToken);

            //Find the containers to stop (there should only be one if all has gone to plan)
            var containersToStop = containers
                .Where(c => string.Equals(c.ImageID, imageId))
                .ToArray();

            if (containersToStop.Any())
            {
                var stopParameters = new ContainerStopParameters
                {
                    WaitBeforeKillSeconds = 30
                };

                //Look at each 
                foreach (var container in containersToStop)
                {
                    Console.WriteLine($"Stopping container '{container.ID}'...");

                    //Stop this application
                    await client.Containers.StopContainerAsync(container.ID, stopParameters, cancellationToken);

                    Console.WriteLine($"Container '{container.ID}' stopped.");
                }
            }
        }

        public static async Task<ImagesListResponse> GetImageAsync(
            this IDockerClient dockerClient,
            string imageId,
            CancellationToken cancellationToken)
        {
            var images = await dockerClient.Images.ListImagesAsync(new ImagesListParameters
            {
                All = true
            }, cancellationToken);

            return images.FirstOrDefault(i => i.ID == imageId);
        }

        public static async Task<bool> IsContainerRunningAsync(this IDockerClient client, string containerId,
            CancellationToken cancellationToken)
        {
            var runningContainers =
                await client.Containers.ListContainersAsync(new ContainersListParameters(), cancellationToken);

            return runningContainers.Any(c => c.ID == containerId);
        }

        public static async Task<int> GetNumberOfRunningContainersAsync(this IDockerClient client, string imageId,
            CancellationToken cancellationToken)
        {
            var runningContainers =
                await client.Containers.ListContainersAsync(new ContainersListParameters(), cancellationToken);

            return runningContainers.Count(c => c.ImageID == imageId);
        }

        public static async Task<bool> DoesImageExistAsync(this IDockerClient client, string imageId,
            CancellationToken cancellationToken)
        {
            var image = await client.GetImageAsync(imageId, cancellationToken);

            return image != null;
        }

        public static async Task<ContainerListResponse> GetContainerByImageId(this IDockerClient client, string imageId,
            CancellationToken cancellationToken)
        {
            var allContainers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            }, cancellationToken);

            return allContainers.FirstOrDefault(c => c.ImageID == imageId);
        }

        public static async Task<ContainerListResponse> GetContainerByName(this IDockerClient client, string name,
            CancellationToken cancellationToken)
        {
            var allContainers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            }, cancellationToken);

            return allContainers.FirstOrDefault(c => c.Names.Any(n => n.EndsWith(name)));
        }
    }
}