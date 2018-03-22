namespace Boondocks.Agent.Base
{
    public static class DockerContainerNames
    {
        /// <summary>
        /// The bootrapper.
        /// </summary>
        public const string Bootstrap = "resin_supervisor";

        /// <summary>
        /// The A name for the agent. We'll flip back and forth.
        /// </summary>
        public const string AgentA = "boondocks-agent-a";

        /// <summary>
        /// The B name for the agent. We'll flip back and forth.
        /// </summary>
        public const string AgentB = "boondocks-agent-b";

        /// <summary>
        /// The name of the application container.
        /// </summary>
        public const string Application = "boondocks-application";

    }
}