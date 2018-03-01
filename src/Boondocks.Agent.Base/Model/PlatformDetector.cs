namespace Boondocks.Agent.Base.Model
{
    using System;
    using Interfaces;

    public class PlatformDetector : IPlatformDetector
    {
        public bool IsLinux => Environment.OSVersion.Platform == PlatformID.Unix;
    }
}