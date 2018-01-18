using System.Threading;
using System.Threading.Tasks;

namespace Boondocks.Agent.Interfaces
{
    internal interface IAgentHost
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}