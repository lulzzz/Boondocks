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

        protected ILogger Logger { get; }

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