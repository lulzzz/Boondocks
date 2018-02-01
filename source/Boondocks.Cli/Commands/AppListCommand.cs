namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.WebApiClient;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("app-list", HelpText = "List the applications.")]
    public class AppListCommand : CommandBase
    {
        [Option('t', "device-type", HelpText = "The device type to filter on.")]
        public string DeviceTypeId { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            var deviceTypeId = DeviceTypeId.TryParseGuid();

            var request = new GetApplicationsRequest
            {
                DeviceTypeId = deviceTypeId
            };

            var applications = await context.Client.Applications.GetApplicationsAsync(request, cancellationToken);

            applications.DisplayEntities(a => $"{a.Id}: {a.Name}");

            return 0;
        }
    }
}