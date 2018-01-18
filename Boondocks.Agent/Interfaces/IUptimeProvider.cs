using System;

namespace Boondocks.Agent.Interfaces
{
    public interface IUptimeProvider
    {
        TimeSpan Ellapsed { get; }
    }
}