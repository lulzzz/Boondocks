namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("app-logs", HelpText = "Displays the console log for the application on the specified device.")]
    public class AppLogs : CommandBase
    {
        [Option('d', "device", Required = true, HelpText = "The device to update.")]
        public string Device { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //Get the device
            var device = await context.FindDeviceAsync(Device, cancellationToken);

            if (device == null)
            {
                return 1;
            }
            
            var entities = await context.Client.ApplicationLogs.GetApplicationLogsAsync(device.Id, cancellationToken);

            entities.DisplayEntities(e => $"{e.CreatedLocal} {e.Message}");

            return 0;
        }
    }
}