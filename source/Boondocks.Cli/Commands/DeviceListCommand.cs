namespace Boondocks.Cli.Commands
{
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.WebApiClient;

    [Verb("device-list", HelpText = "List devices.")]
    public class DeviceListCommand : CommandBase
    {
        [Option('a', "app-id", HelpText = "The id of the application to filter on.")]
        public string ApplicationId { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            var request = new GetDevicesRequest
            {
                ApplicationId = ApplicationId.TryParseGuid()
            };

            var devices = await context.Client.Devices.GetDevicesAsync(request);

            devices.DisplayEntities(d => $"{d.Id:D}: {d.Name}");

            return 0;
        }
    }
}