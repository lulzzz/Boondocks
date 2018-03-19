namespace Boondocks.Agent.Base.Interfaces
{
    public interface IEnvironmentConfigurationProvider
    {
        string DockerSocket { get; }

        string BootMountpoint { get; }
    }
}