using System;
using System.Diagnostics;

namespace Boondocks.Supervisor.Model
{
    public class UptimeProvider
    {
        private readonly Stopwatch _stopwatch;

        public UptimeProvider()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public TimeSpan Ellapsed => _stopwatch.Elapsed;
    }
}