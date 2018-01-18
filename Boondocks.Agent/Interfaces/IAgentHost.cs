using System.Threading;
using System.Threading.Tasks;

namespace Boondocks.Agent.Interfaces
{
    /// <summary>
    /// The core code for the agent.
    /// </summary>
    internal interface IAgentHost
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}