using System;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using Boondocks.Services.Management.WebApiClient;
using BoondocksCli.ExtensionMethods;
using CommandLine;

namespace BoondocksCli.Commands
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
                ApplicationId = ApplicationId.ParseGuid()
            };

            var applicationVersions = await context.Client.GetApplicationVersionsAsync(request);

            applicationVersions.DisplayEntities(v => $"{v.Id}: {v.Name}");

            return 0;
        }
    }
}