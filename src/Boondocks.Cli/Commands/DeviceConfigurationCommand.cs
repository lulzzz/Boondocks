namespace Boondocks.Cli.Commands
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("device-config", HelpText = "Download the configuration file for a given device.")]
    public class DeviceConfigurationCommand : CommandBase
    {
        [Option('d', "device", Required = true, HelpText = "The device name or uuid.")]
        public string Device { get; set; }

        [Option('o', "output", Required = true, HelpText = "The name of the file to output the configuration to.")]
        public string Output { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //Get the device
            var device = await context.FindDeviceAsync(Device, cancellationToken);

            if (device == null)
            {
                return 1;
            }

            //Get the configuration
            var configuration = await context.Client.DeviceConfiguration.GetDeviceConfigurationAsync(device.Id, cancellationToken);

            //Write out the file.
            await File.WriteAllTextAsync(Output, configuration, cancellationToken);

            return 0;
        }
    }
}