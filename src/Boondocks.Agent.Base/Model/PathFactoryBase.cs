namespace Boondocks.Agent.Base.Model
{
    public abstract class PathFactoryBase : IPathFactory
    {
        protected const string DeviceConfigFilename = "boondocks.device.config";

        public abstract string DeviceConfigFile { get; }

        public abstract string DockerEndpoint { get; }
    }
}