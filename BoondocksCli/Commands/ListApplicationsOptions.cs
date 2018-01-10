using System;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using Boondocks.Services.Management.WebApiClient;
using BoondocksCli.ExtensionMethods;
using CommandLine;

namespace BoondocksCli.Commands
{
    [Verb("list-apps", HelpText = "List the applications.")]
    public class ListApplicationsOptions :  OptionsBase
    {
        [Option('t', "device-type", HelpText = "The device type to filter on.")]
        public string DeviceTypeId { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            Guid? deviceTypeId = DeviceTypeId.ParseGuid();

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