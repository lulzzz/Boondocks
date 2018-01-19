using System;
using Boondocks.Agent.Interfaces;

namespace Boondocks.Agent.Model
{
    internal class PlatformDetector : IPlatformDetector
    {
        public bool IsLinux
        {
            get { return Environment.OSVersion.Platform == PlatformID.Unix; }
        }
    }
}