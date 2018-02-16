namespace Boondocks.Agent
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;

    internal abstract class UpdateService
    {
        protected UpdateService(ILogger logger)
        {
            Logger = logger.ForContext(GetType());
        }

        public abstract bool IsUpdatePending();
        
        protected ILogger Logger { get; }

        protected async Task StopAndDestroyApplicationAsync(IDockerClient dockerClient, string name, CancellationToken cancellationToken)
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
                Logger.Information("Removing application container {ContainerId} with image {ImageId}", container.ID, container.ImageID);

                //Delete it
                await dockerClient.Containers.RemoveContainerAsync(container.ID, parameters, cancellationToken);
            }
        }

        public async Task<bool> UpdateAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (IsUpdatePending())
                {

                    return await UpdateCoreAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "An error occurred during update.");
            }

            return false;
        }

        public abstract Task<bool> UpdateCoreAsync(CancellationToken cancellationToken);
    }
}