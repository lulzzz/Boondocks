namespace Boondocks.Agent.Model
{
    /// <summary>
    /// Responsible for creating a docker container for an application.
    /// </summary>
    internal class ApplicationDockerContainerFactory : DockerContainerFactory
    {
        protected override string CreateName()
        {
            return DockerConstants.ApplicationContainerName;
        }
    }
}