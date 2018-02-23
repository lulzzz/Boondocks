namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("device-var-list", HelpText = "Lists the device environment variables.")]
    public class DeviceVarListCommand : CommandBase
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

            var variables =
                await context.Client.DeviceEnvironmentVariables.GetEnvironmentVariables(device.Id,
                    cancellationToken);

            variables.DisplayEntities(v => $"{v.Id}: {v.Name}={v.Value}");

            return 0;
        }
    }
}