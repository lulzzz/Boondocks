using System;

namespace Boondocks.Supervisor.Model
{
    public class PlatformDetector
    {
        public bool IsLinux
        {
            get { return Environment.OSVersion.Platform == PlatformID.Unix; }
        }
    }
}