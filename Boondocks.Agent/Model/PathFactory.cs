using System;
using System.IO;
using Boondocks.Agent.Interfaces;

namespace Boondocks.Agent.Model
{
    internal class PathFactory
    {
        public PathFactory(IPlatformDetector platformDetector)
        {
            if (platformDetector == null) throw new ArgumentNullException(nameof(platformDetector));

            Root = platformDetector.IsLinux ? "/" : @"c:\Boondocks\";
        }

        public string Root { get; }

        public string Boot => Path.Combine(Root, "boot");

        public string DeviceConfigFile => Path.Combine(Boot, "device.config");

        public string SupervisorStatusDirectory => Path.Combine(Root, "boondocks-status");

        public string OperationStatePath => Path.Combine(Root, SupervisorStatusDirectory, "dev-op-state.json");
    }
}