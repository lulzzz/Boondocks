using System.Threading.Tasks;
using Boondocks.Cli.ExtensionMethods;
using Boondocks.Services.Base;
using Boondocks.Services.Management.WebApiClient;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("device-list", HelpText = "List devices.")]
    public class DeviceListCommand : CommandBase
    {
        [Option('a', "app-id", HelpText = "The id of the application to filter on.")]
        public string ApplicationId { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            var request = new GetDevicesRequest
            {
                ApplicationId = ApplicationId.TryParseGuid()
            };

            var devices = await context.Client.GetDevicesAsync(request);

            devices.DisplayEntities(d => $"{d.Id:D}: {d.Name}");

            return 0;
        }
    }
}