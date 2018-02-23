namespace Boondocks.Cli.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("app-var-delete", HelpText = "Delete an application environment variable.")]
    public class AppVarDeleteCommand : CommandBase
    {
        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        [Option('n', "name", Required = true, HelpText = "The name of the variable.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            // Get the application
            var application = await context.FindApplicationAsync(Application, cancellationToken);

            if (application == null)
                return 1;

            //Get the existing variables
            var variables = await context.Client.ApplicationEnvironmentVariables.GetEnvironmentVariables(application.Id, cancellationToken);

            var variable = variables.FirstOrDefault(v => v.Name == Name);

            if (variable == null)
            {
                Console.WriteLine($"Unable to find variable '{Name}' for application '{application.Name}'.");
                return 1;
            }
            else
            {
                //Update the existing one
                await context.Client.ApplicationEnvironmentVariables.DeleteEnvironmentVariable(variable.Id, cancellationToken);
            }

            return 0;
        }
    }
}