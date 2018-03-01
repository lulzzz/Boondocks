namespace Boondocks.Agent.Base.Model
{
    using System;
    using System.Diagnostics;
    using Interfaces;

    internal class UptimeProvider : IUptimeProvider
    {
        private readonly Stopwatch _stopwatch;

        public UptimeProvider()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public TimeSpan Ellapsed => _stopwatch.Elapsed;
    }
}