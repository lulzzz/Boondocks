namespace Boondocks.Agent.Base.Model
{
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;

    internal abstract class DockerContainerFactory
    {
        protected DockerContainerFactory(ILogger logger)
        {
            Logger = logger.ForContext(GetType());
        }

        protected ILogger Logger { get; }

        public abstract Task<CreateContainerResponse> CreateContainerAsync(IDockerClient dockerClient, string imageId,
            CancellationToken cancellationToken);
    }
}