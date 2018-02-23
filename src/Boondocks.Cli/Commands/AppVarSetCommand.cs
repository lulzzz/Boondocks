namespace Boondocks.Cli.Commands
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.Contracts;

    [Verb("app-var-set", HelpText = "Sets an application environment variable.")]
    public class AppVarSetCommand : CommandBase
    {
        [Option('n', "name", Required = true, HelpText = "The name of the variable.")]
        public string Name { get; set; }

        [Option('v', "value", Required = true, HelpText = "The value of the variable.")]
        public string Value { get; set; }

        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

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
                var request = new CreateApplicationEnvironmentVariableRequest()
                {
                    ApplicationId = application.Id,
                    Name = Name,
                    Value = Value
                };

                //Create the new one
                await context.Client.ApplicationEnvironmentVariables.CreateEnvironmentVariableAsync(request,
                    cancellationToken);
            }
            else
            {
                //Create a new one
                variable.Value = Value;

                //Update the existing one
                await context.Client.ApplicationEnvironmentVariables.UpdateEnvironmentVariable(variable, cancellationToken);
            }

            return 0;
        }
    }
}