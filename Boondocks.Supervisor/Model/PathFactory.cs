using System;

namespace Boondocks.Supervisor.Model
{
    public class PathFactory
    {
        private readonly string _root;

        public PathFactory(PlatformDetector platformDetector)
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


    }
}