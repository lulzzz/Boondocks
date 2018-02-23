namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("app-var-list", HelpText = "Lists the application environment variables.")]
    public class AppVarListCommand : CommandBase
    {
        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            //Get the application
            var application = await context.FindApplicationAsync(Application, cancellationToken);

            if (application == null)
                return 1;

            var variables =
                await context.Client.ApplicationEnvironmentVariables.GetEnvironmentVariables(application.Id,
                    cancellationToken);

            variables.DisplayEntities(v => $"{v.Id}: {v.Name}={v.Value}");

            return 0;
        }
    }
}