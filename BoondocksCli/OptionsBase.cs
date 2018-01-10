using System.Threading.Tasks;

namespace BoondocksCli
{
    public abstract class OptionsBase
    {
        public abstract Task<int> ExecuteAsync(ExecutionContext context);

    }
}