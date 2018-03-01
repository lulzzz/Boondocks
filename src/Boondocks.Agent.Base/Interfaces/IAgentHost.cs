namespace Boondocks.Agent.Base.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     The core code for the agent.
    /// </summary>
    public interface IAgentHost
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}