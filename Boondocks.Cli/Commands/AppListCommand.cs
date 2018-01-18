using System;
using System.Threading.Tasks;
using Boondocks.Cli.ExtensionMethods;
using Boondocks.Services.Base;
using Boondocks.Services.Management.WebApiClient;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("app-list", HelpText = "List the applications.")]
    public class AppListCommand :  CommandBase
    {
        [Option('t', "device-type", HelpText = "The device type to filter on.")]
        public string DeviceTypeId { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            Guid? deviceTypeId = DeviceTypeId.TryParseGuid();

            var request = new GetApplicationsRequest()
            {
                DeviceTypeId = deviceTypeId
            };

            var applications = await context.Client.GetApplicationsAsync(request);

            applications.DisplayEntities(a => $"{a.Id}: {a.Name}");

            return 0;
        }
    }
}