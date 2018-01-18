using System;
using System.Diagnostics;
using Boondocks.Agent.Interfaces;

namespace Boondocks.Agent.Model
{
    public class UptimeProvider : IUptimeProvider
    {
        private readonly Stopwatch _stopwatch;

        public UptimeProvider()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public TimeSpan Ellapsed => _stopwatch.Elapsed;
    }
}