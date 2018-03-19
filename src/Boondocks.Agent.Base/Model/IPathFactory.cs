namespace Boondocks.Agent.Base.Model
{
    public interface IPathFactory
    {
        /// <summary>
        /// Gets the path to the boondocks.device.config file
        /// </summary>
        string DeviceConfigFile { get; }

        /// <summary>
        /// Gets the docker endpoint for the current platform
        /// </summary>
        string DockerEndpoint { get; }
    }
}