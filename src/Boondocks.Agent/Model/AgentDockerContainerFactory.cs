namespace Boondocks.Agent.Model
{
    internal class AgentDockerContainerFactory : DockerContainerFactory
    {
        protected override string CreateName()
        {
            return DockerConstants.AgentContainerName;
        }
    }
}