namespace Boondocks.Agent.Shared
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;
    using Serilog.Core;

    public static class DockerClientExtensions
    {
        /// <summary>
        /// Removes a container (by name) using the Force option.
        /// </summary>
        /// <param name="dockerClient"></param>
        /// <param name="name"></param>
        /// <param name="logger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task ObliterateContainerAsync(this IDockerClient dockerClient, string name, ILogger logger = null, CancellationToken cancellationToken = new CancellationToken())
        {
            //Get all of the containers
            var containers = await dockerClient.Containers.ListContainersAsync(new ContainersListParameters()
            {
                All = true
            }, cancellationToken);

            //Find all of the application containers (should should be one)
            var containersToDelete = containers
                .Where(c => c.Names.Any(n => n.EndsWith(name)))
                .ToArray();

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
    }
}