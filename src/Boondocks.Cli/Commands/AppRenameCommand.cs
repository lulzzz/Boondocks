namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("app-rename", HelpText = "Rename an application")]
    public class AppRenameCommand : CommandBase
    {
        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        [Option('n', "name", Required = true, HelpText = "The new name of the application.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //Get the application
            var application = await context.FindApplicationAsync(Application, cancellationToken);

            if (application == null)
                return 1;

            if (string.IsNullOrWhiteSpace(Name))
            {
                Console.WriteLine("No name was specified.");
                return 1;
            }

            //Change the name
            application.Name = Name;

            //Update it!
            await context.Client.Applications.UpdateApplicationAsync(application, cancellationToken);

            //We're done here.
            Console.WriteLine("Application renamed.");

            return 0;
        }
    }
}