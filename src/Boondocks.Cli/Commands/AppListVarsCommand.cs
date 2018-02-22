namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("app-list-vars", HelpText = "Lists the application environment variables.")]
    public class AppListVarsCommand : CommandBase
    {
        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        protected override Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}