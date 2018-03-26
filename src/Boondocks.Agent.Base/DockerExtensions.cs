namespace Boondocks.Agent.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;

    public static class DockerExtensions
    {
        /// <summary>
        /// Gets an image given its id. 
        /// </summary>
        /// <param name="dockerClient"></param>
        /// <param name="imageId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The image if it is found, null otherwise.</returns>
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

        /// <summary>
        /// Deteremines if an image has been downloaded.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="imageId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<bool> DoesImageExistAsync(this IDockerClient client, string imageId,
            CancellationToken cancellationToken)
        {
            var image = await client.GetImageAsync(imageId, cancellationToken);

            return image != null;
        }

        /// <summary>
        /// Gets the first container with the given image id.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="imageId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<ContainerListResponse> GetContainerByImageId(this IDockerClient client, string imageId,
            CancellationToken cancellationToken)
        {
            var allContainers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            }, cancellationToken);

            return allContainers.FirstOrDefault(c => c.ImageID == imageId);
        }

        /// <summary>
        /// Attemmpts to get a container by its name.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The container if it is found, null otherwise.</returns>
        public static async Task<ContainerListResponse> GetContainerByName(this IDockerClient client, string name,
            CancellationToken cancellationToken)
        {
            var allContainers = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            }, cancellationToken);

            return allContainers.FindByName(name);
        }

          /// <summary>
        /// Removes a container (by name) using the Force option.
        /// </summary>
        /// <param name="dockerClient"></param>
        /// <param name="name"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ObliterateContainerAsync(this IDockerClient dockerClient, string name,
            ILogger logger = null, CancellationToken cancellationToken = new CancellationToken())
        {
            //Get all of the containers
            var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true
            }, cancellationToken);

            var container = containers.FindByName(name);

            if (container != null)
            {
                //Create the parameters
                var parameters = new ContainerRemoveParameters()
                {
                    Force = true
                };

                logger?.Information("Obliterating container {ContainerId} with image {ImageId}", container.ID,
                    container.ImageID);

                //Delete it
                await dockerClient.Containers.RemoveContainerAsync(container.ID, parameters, cancellationToken);
            }
        }

        /// <summary>
        /// Removes a container (by name) using the Force option.
        /// </summary>
        /// <param name="dockerClient"></param>
        /// <param name="names"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ObliterateContainersAsync(this IDockerClient dockerClient, string[] names, ILogger logger = null, CancellationToken cancellationToken = new CancellationToken())
        {
            //Get all of the containers
            var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true
            }, cancellationToken);

            //Find all of the application containers (should should be one)
            var containersToDelete = containers.FindByNames(names);

            //Create the parameters
            var parameters = new ContainerRemoveParameters()
            {
                Force = true
            };

            //Delete each one
            foreach (var container in containersToDelete)
            {
                logger?.Information("Removing application container {ContainerId} with image {ImageId}", container.ID, container.ImageID);

                //Delete it
                await dockerClient.Containers.RemoveContainerAsync(container.ID, parameters, cancellationToken);
            }
        }

        /// <summary>
        /// Removes the leading "/" from a container name.
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static string GetContainerName(string containerName)
        {
            if (!string.IsNullOrWhiteSpace(containerName) && containerName.StartsWith("/"))
                return containerName.Substring(1);

            return containerName;
        }

        /// <summary>
        /// Gets the friendly formatted names of a given container.
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetContainerNames(this ContainerListResponse container)
        {
            return container.Names
                .Select(GetContainerName);
        }

        /// <summary>
        /// True if the container has the specified name, false otherwise.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasName(this ContainerListResponse container, string name)
        {
            return container
                .GetContainerNames()
                .Contains(name);
        }

        /// <summary>
        /// Gets a container by name. 
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="name"></param>
        /// <returns>The container if it's found, null otherwise.</returns>
        public static ContainerListResponse FindByName(this IEnumerable<ContainerListResponse> containers, string name)
        {
            return containers.FirstOrDefault(c => c.HasName(name));
        }

        /// <summary>
        /// Gets containers by name.
        /// </summary>
        /// <param name="containers"></param>
        /// <param name="names"></param>
        /// <returns>The containers if found, emtpy list otherwise.</returns>
        public static IList<ContainerListResponse> FindByNames(this IEnumerable<ContainerListResponse> containers, IList<string> names)
        {
            var response = new List<ContainerListResponse>();

            foreach (var container in containers)
            {
                foreach (var name in names)
                {
                    if (container.HasName(name))
                    {
                        response.Add(container);
                    }
                }
            }

            return response;
        }
    }
}