using System.Threading.Tasks;
using Boondocks.Cli.ExtensionMethods;
using Boondocks.Services.Base;
using Boondocks.Services.Management.WebApiClient;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("app-version-list", HelpText = "Lists the available application versions.")]
    public class AppVersionListCommand : CommandBase
    {
        [Option('a', "app-id", Required = true, HelpText = "The id of the application to filter on.")]
        public string ApplicationId { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {

            var request = new GetApplicationVersionsRequest
            {
                ApplicationId = ApplicationId.TryParseGuid()
            };

            var applicationVersions = await context.Client.GetApplicationVersionsAsync(request);

            applicationVersions.DisplayEntities(v => $"{v.Id}: {v.Name}");

            return 0;
        }
    }
}