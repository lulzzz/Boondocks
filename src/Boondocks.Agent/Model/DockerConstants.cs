﻿namespace Boondocks.Agent.Model
{
    public static class DockerConstants
    {
        public const string ApplicationContainerName = "boondocks-application";

        /// <summary>
        /// The name of the main agent instance.
        /// </summary>
        public const string AgentContainerName = "boondocks-agent";

        /// <summary>
        /// We temporarily rename the agent container before deleting it (during the update process).
        /// </summary>
        public const string AgentContainerOutgoingName = "boondocks-agent-outgoing";
    }
}