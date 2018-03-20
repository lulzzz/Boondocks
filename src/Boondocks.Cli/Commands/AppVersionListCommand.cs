namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.WebApiClient;

    [Verb("app-ver-list", HelpText = "Lists the available application versions.")]
    public class AppVersionListCommand : CommandBase
    {
        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //Get the application
            var application = await context.FindApplicationAsync(Application, cancellationToken);
           
            if (application == null)
                return 1;
            
            //Create the request
            var request = new GetApplicationVersionsRequest
            {
                ApplicationId = application.Id
            };

            //Get the versions
            var applicationVersions = await context.Client.ApplicationVersions.GetApplicationVersionsAsync(request, cancellationToken);

            //Display them.
            applicationVersions.DisplayEntities(v => $"{v.Id}: {v.Name}");

            return 0;
        }
    }
}