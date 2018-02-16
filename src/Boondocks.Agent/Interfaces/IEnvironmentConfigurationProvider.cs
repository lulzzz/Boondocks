namespace Boondocks.Agent.Interfaces
{
    public interface IEnvironmentConfigurationProvider
    {
        string AgentVersion { get; }

        string DockerEndpoint { get; }
    }
}