namespace Boondocks.Agent.Base
{
    using System;
    using Interfaces;
    using Model;

    public static class PlatformDetectorExtensions
    {
        public static IPathFactory CreatePathFactory(this IPlatformDetector platformDetector)
        {
            if (platformDetector.IsLinux)
            {
                return new LinuxPathFactory();
            }
            
            Console.WriteLine("WARNING: Running on Windows.");

            return new WindowsPathFactory();
        }
    }
}