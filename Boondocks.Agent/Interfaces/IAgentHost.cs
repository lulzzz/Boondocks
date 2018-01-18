using System.Threading;
using System.Threading.Tasks;

namespace Boondocks.Agent.Interfaces
{
    public interface IAgentHost
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}