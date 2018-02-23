namespace Boondocks.Cli
{
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;

    public abstract class CommandBase
    {
        //TODO: Change the default for this to the public endpoint.
        [Option('e', "endpoint-url", HelpText = "Specify the management API endpoint.",
            Default = "http://localhost/Boondocks.Services.Management.WebApi/")]
        public string EndpointUrl { get; set; }

        protected abstract Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken);

        public Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            var context = new CommandContext(EndpointUrl);

            return ExecuteAsync(context, cancellationToken);
        }
    }
}