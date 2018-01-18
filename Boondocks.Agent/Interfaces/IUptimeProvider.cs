using System;

namespace Boondocks.Agent.Interfaces
{
    internal interface IUptimeProvider
    {
        TimeSpan Ellapsed { get; }
    }
}