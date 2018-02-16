namespace Boondocks.Agent.Model
{
    using System;
    using Interfaces;

    internal class PlatformDetector : IPlatformDetector
    {
        public bool IsLinux => Environment.OSVersion.Platform == PlatformID.Unix;
    }
}