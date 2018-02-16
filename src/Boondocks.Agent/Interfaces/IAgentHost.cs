namespace Boondocks.Agent.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     The core code for the agent.
    /// </summary>
    internal interface IAgentHost
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}