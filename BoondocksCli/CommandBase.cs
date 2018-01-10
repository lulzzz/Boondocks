using System.Threading.Tasks;

namespace BoondocksCli
{
    public abstract class CommandBase
    {
        public abstract Task<int> ExecuteAsync(ExecutionContext context);

    }
}