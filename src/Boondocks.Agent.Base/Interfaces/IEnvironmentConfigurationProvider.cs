namespace Boondocks.Agent.Base.Interfaces
{
    public interface IEnvironmentConfigurationProvider
    {
        string AgentVersion { get; }

        string DockerEndpoint { get; }
    }
}