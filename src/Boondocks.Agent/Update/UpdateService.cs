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

    internal abstract class UpdateService
    {
        private VersionReference _nextVersion;

        protected UpdateService(ILogger logger)
        {
            Logger = logger.ForContext(GetType());
        }

        public abstract Task<string> GetCurrentVersionAsync();
       
        protected ILogger Logger { get; }

        public abstract VersionReference GetVersionFromConfiguration(GetDeviceConfigurationResponse response);

        /// <summary>
        /// Compare this version to the current one to see if we need to update.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public async Task ProcessConfigurationAsync(GetDeviceConfigurationResponse response)
        {
            var newVersion = GetVersionFromConfiguration(response);

            if (newVersion == null)
            {
                Logger.Verbose("No configuration information available.");                
            }
            else
            {
                //Get the current version
                var currentVersion = await GetCurrentVersionAsync();

                if (currentVersion == newVersion.ImageId)
                {
                    Logger.Verbose("The version is the same: {ImageId}", newVersion.ImageId);
                }
                else
                {
                    Logger.Verbose("A new version was found: {ImageId}", newVersion.ImageId);

                    //Save this so we can install it.
                    _nextVersion = newVersion;
                }
            }
        }

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
                if (_nextVersion != null)
                {
                    var result = await UpdateCoreAsync(_nextVersion, cancellationToken);

                    _nextVersion = null;

                    return result;
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "An error occurred during update.");
            }

            return false;
        }

        public abstract Task<bool> UpdateCoreAsync(VersionReference imageId, CancellationToken cancellationToken);
    }
}