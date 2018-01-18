using System.Threading.Tasks;

namespace Boondocks.Cli
{
    public abstract class CommandBase
    {
        public abstract Task<int> ExecuteAsync(ExecutionContext context);

    }
}