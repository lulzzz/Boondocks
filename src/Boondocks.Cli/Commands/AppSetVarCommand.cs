namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("app-set-var", HelpText = "Sets an application environment variable.")]
    public class AppSetVarCommand : CommandBase
    {
        [Option('n', "name", Required = true, HelpText = "The name of the variable.")]
        public string Name { get; set; }

        [Option('v', "value", Required = true, HelpText = "The value of the variable.")]
        public string Value { get; set; }

        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            //Get the current variables
            throw new NotImplementedException();
        }
    }
}