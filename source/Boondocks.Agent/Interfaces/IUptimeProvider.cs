namespace Boondocks.Agent.Interfaces
{
    using System;

    /// <summary>
    ///     Provides the amount of time that the agent has been running.
    /// </summary>
    internal interface IUptimeProvider
    {
        TimeSpan Ellapsed { get; }
    }
}