using System;
using System.IO;
using Boondocks.Agent.Interfaces;

namespace Boondocks.Agent.Model
{
    public class PathFactory
    {
        private readonly string _root;

        public PathFactory(IPlatformDetector platformDetector)
        {
            if (platformDetector == null) throw new ArgumentNullException(nameof(platformDetector));

            if (platformDetector.IsLinux)
            {
                _root = "/";
            }
            else
            {
                _root = @"c:\Boondocks\";
            }
        }

        public string Root => _root;

        public string SupervisorStatusDirectory => Path.Combine(Root, "boondocks-status");

        public string OperationStatePath => Path.Combine(Root, SupervisorStatusDirectory, "dev-op-state.json");
    }
}