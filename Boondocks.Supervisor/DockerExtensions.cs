using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Boondocks.Supervisor
{
    public static class DockerExtensions
    {
        public static async Task StartAllContainers(this DockerClient client)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true
            });

            foreach (var container in containers)
            {
                Console.WriteLine($"Starting container {container.ID}...");

                await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters()
                {

                });
            }
        }

        public static async Task RemoveAllContainersAsync(this DockerClient client)
        {
            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true
            });

            foreach (var container in containers)
            {
                Console.WriteLine($"Deleting container {container.ID}...");

                await client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters()
                {
                    Force = true
                });
            }
        }

        public static async Task DeleteAllImagesAsync(this DockerClient client)
        {
            var images = await client.Images.ListImagesAsync(new ImagesListParameters()
            {
                All = true
            });

            foreach (var image in images)
            {
                Console.WriteLine($"Deleting image '{image.ID}'...");

                await client.Images.DeleteImageAsync(image.ID, new ImageDeleteParameters()
                {
                    Force = true,
                    PruneChildren = true
                });
            }
        }
    }
}