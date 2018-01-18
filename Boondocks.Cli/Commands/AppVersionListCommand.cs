using System.Threading.Tasks;
using Boondocks.Cli.ExtensionMethods;
using Boondocks.Services.Base;
using Boondocks.Services.Management.WebApiClient;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("app-version-list")]
    public class AppVersionListCommand : CommandBase
    {
        [Option('a', "app-id", HelpText = "The id of the application to filter on.")]
        public string ApplicationId { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
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