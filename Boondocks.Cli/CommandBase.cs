using System.Threading.Tasks;
using CommandLine;

namespace Boondocks.Cli
{
    public abstract class CommandBase
    {
        protected abstract Task<int> ExecuteAsync(ExecutionContext context);

        public Task<int> ExecuteAsync()
        {
            var context = new ExecutionContext(EndpointUrl);

            return ExecuteAsync(context);
        }

        [Option('e', "endpoint-url", HelpText = "Specify a specific management endpoint.", Default = "http://localhost:54985/")]
        public string EndpointUrl { get; set; }
    }
}