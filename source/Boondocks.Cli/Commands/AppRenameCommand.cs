namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("app-rename", HelpText = "Rename an application")]
    public class AppRenameCommand : CommandBase
    {
        [Option('a', "app-id", Required = true, HelpText = "The application uuid.")]
        public string ApplicationId { get; set; }

        [Option('n', "name", Required = true, HelpText = "The new name of the application.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            //Console.WriteLine($"Deploy app: {ApplicationId} {ImageId}");
            var applicationId = ApplicationId.TryParseGuid();

            if (applicationId == null)
            {
                Console.WriteLine("No valid application id was specified.");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(Name))
            {
                Console.WriteLine("No name was specified.");
                return 1;
            }

            //Get the existing application
            var application = await context.Client.Applications.GetApplicationAsync(applicationId.Value, cancellationToken);

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