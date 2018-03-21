namespace Boondocks.Agent.Base
{
    public static class DockerContainerNames
    {
        /// <summary>
        /// The name of the applicatoin container.
        /// </summary>
        public const string Application = "boondocks-application";

        /// <summary>
        /// We temporarily rename the agent container before deleting it (during the update process).
        /// </summary>
        public const string AgentOutgoing = "boondocks-agent-outgoing";

        /// <summary>
        /// We create the new agent container with a temporary name in case it doesn't start.
        /// </summary>
        public const string AgentIncoming = "boondocks-agent-incoming";

        /// <summary>
        /// The name of the main agent instance. We use the same name as resin to maintain compatability with the resin YOCTO build.
        /// </summary>
        public const string Agent = "resin_supervisor";
    }
}