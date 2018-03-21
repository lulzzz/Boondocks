namespace Boondocks.Agent.Base
{
    public static class DockerContainerNames
    {
        /// <summary>
        /// The name of the applicatoin container.
        /// </summary>
        public const string Application = "boondocks-application";

        /// <summary>
        /// The name of the main agent instance. We use the same name as resin to maintain compatability with the resin YOCTO build.
        /// </summary>
        public const string AgentA = "resin_supervisor";

        /// <summary>
        /// The B name for the agent. We'll flip back and forth.
        /// </summary>
        public const string AgentB = "resin_supervisor_b";
    }
}