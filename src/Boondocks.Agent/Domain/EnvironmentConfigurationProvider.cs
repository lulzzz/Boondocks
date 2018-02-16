namespace Boondocks.Agent.Domain
{
    using System;
    using Interfaces;

    /// <summary>
    /// Pulls configuration values directly from the environment
    /// </summary>
    public class EnvironmentConfigurationProvider : IEnvironmentConfigurationProvider
    {
        /// <summary>
        /// How to connect to the docker (balena) endpoint
        /// </summary>
        public string DockerEndpoint => Environment.GetEnvironmentVariable("DOCKER_ENDPOINT");

        /// <summary>
        /// The version of the boondocks agent installed.
        /// </summary>
        public string AgentVersion => Environment.GetEnvironmentVariable("BOONDOCKS_AGENT_VERSION");
    }
}