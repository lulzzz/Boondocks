namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.WebApiClient;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("app-version-list", HelpText = "Lists the available application versions.")]
    public class AppVersionListCommand : CommandBase
    {
        [Option('a', "app-id", Required = true, HelpText = "The id of the application to filter on.")]
        public string ApplicationId { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            var request = new GetApplicationVersionsRequest
            {
                ApplicationId = ApplicationId.TryParseGuid()
            };

            var applicationVersions = await context.Client.ApplicationVersions.GetApplicationVersionsAsync(request, cancellationToken);

            applicationVersions.DisplayEntities(v => $"{v.Id}: {v.Name}");

            return 0;
        }
    }
}