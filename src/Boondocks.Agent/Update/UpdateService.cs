namespace Boondocks.Agent.Update
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;
    using Services.Device.Contracts;

    /// <summary>
    /// Base class for update services.
    /// </summary>
    internal abstract class UpdateService
    {
        private readonly IDockerClient _dockerClient;

        protected UpdateService(ILogger logger, IDockerClient dockerClient)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            Logger = logger.ForContext(GetType());
        }

//        public abstract Task<string> GetCurrentVersionAsync();
       
        protected ILogger Logger { get; }

       // public abstract VersionReference GetVersionFromConfiguration(GetDeviceConfigurationResponse response);

        ///// <summary>
        ///// Compare this version to the current one to see if we need to update.
        ///// </summary>
        ///// <param name="response"></param>
        ///// <returns></returns>
        //public async Task ProcessConfigurationAsync(GetDeviceConfigurationResponse response)
        //{
        //    var newVersion = GetVersionFromConfiguration(response);

        //    if (newVersion == null)
        //    {
        //        Logger.Verbose("No configuration information available.");                
        //    }
        //    else
        //    {
        //        //Get the current version
        //        var currentVersion = await GetCurrentVersionAsync();

        //        if (currentVersion == newVersion.ImageId)
        //        {
        //            Logger.Verbose("The version is the same: {ImageId}", newVersion.ImageId);
        //        }
        //        else
        //        {
        //            Logger.Verbose("A new version was found: {ImageId}", newVersion.ImageId);

        //            //Save this so we can install it.
        //            _nextVersion = newVersion;
        //        }
        //    }
        //}

        /// <summary>
        /// Removes a container (by name) using the Force option.
        /// </summary>
        /// <param name="dockerClient"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Removes images that are no longer referenced by a container.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task PruneImagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                //Perform the prune
                var response =  await _dockerClient.Images.PruneImagesAsync(null, cancellationToken);

                Logger.Information("{Count} images pruned for a savings of {Size} bytes.", response?.ImagesDeleted?.Count ?? 0, response?.SpaceReclaimed ?? 0);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "An error occurred while attempting to prune the images: {Message}", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(GetDeviceConfigurationResponse configuration, CancellationToken cancellationToken)
        {
            try
            {
                return await UpdateCoreAsync(configuration, cancellationToken);
                
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "An error occurred during update.");
            }

            return false;
        }

        public abstract Task<bool> UpdateCoreAsync(GetDeviceConfigurationResponse configuration, CancellationToken cancellationToken);
    }
}