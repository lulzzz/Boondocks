using System;
using System.IO;
using Boondocks.Agent.Interfaces;

namespace Boondocks.Agent.Model
{
    /// <summary>
    /// Responsible for generating the paths we use throughout the agent.
    /// </summary>
    internal class PathFactory
    {
        public PathFactory(IPlatformDetector platformDetector)
        {
            if (platformDetector == null) throw new ArgumentNullException(nameof(platformDetector));

            Root = platformDetector.IsLinux ? "/" : @"c:\Boondocks\";
        }

        /// <summary>
        /// The root of the file system.
        /// </summary>
        public string Root { get; }

        /// <summary>
        /// The boot directory
        /// </summary>
        public string Boot => Path.Combine(Root, "boot");

        /// <summary>
        /// The path of the device configuration file.
        /// </summary>
        public string DeviceConfigFile => Path.Combine(Boot, "device.config");

        /// <summary>
        /// The directory in which we put the agent status.
        /// </summary>
        public string AgentStatusDirectory => Path.Combine(Root, "boondocks-status");

        /// <summary>
        /// The path to where we put the device operational state file.
        /// </summary>
        public string OperationStatePath => Path.Combine(Root, AgentStatusDirectory, "dev-op-state.json");
    }
}