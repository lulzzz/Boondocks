namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.WebApiClient;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("device-list", HelpText = "List devices.")]
    public class DeviceListCommand : CommandBase
    {
        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            //Get the application
            var application = await context.FindApplicationAsync(Application, cancellationToken);

            if (application == null)
                return 1;

            var request = new GetDevicesRequest
            {
                ApplicationId = application.Id
            };

            var devices = await context.Client.Devices.GetDevicesAsync(request, cancellationToken);

            devices.DisplayEntities(d => $"{d.Id:D}: {d.Name}");

            return 0;
        }
    }
}