using System;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using Boondocks.Services.Management.WebApiClient;
using BoondocksCli.ExtensionMethods;
using CommandLine;

namespace BoondocksCli.Commands
{
    [Verb("list-devices", HelpText = "List devices.")]
    public class ListDevicesOptions : OptionsBase
    {
        [Option('a', "app-id", HelpText = "The id of the application to filter on.")]
        public string ApplicationId { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            var request = new GetDevicesRequest
            {
                ApplicationId = ApplicationId.ParseGuid()
            };

            var devices = await context.Client.GetDevicesAsync(request);

            devices.DisplayEntities(d => $"{d.Id:D}: {d.Name}");

            return 0;
        }
    }
}