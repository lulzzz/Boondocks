namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExtensionMethods;
    using Services.Contracts;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("device-set-app-version", HelpText = "Sets the current application version for a given device.")]
    public class DeviceSetAppVersion : CommandBase
    {
        [Option('d', "device", Required = true, HelpText = "The device to update.")]
        public string Device { get; set; }

        [Option('v', "app-version-id", HelpText = "The application version to use. Leave blank to use the value specfied at the application level.")]
        public string ApplicationVersion { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            //Get the device
            var device = await context.FindDeviceAsync(Device, cancellationToken);

            if (device == null)
            {
                return 1;
            }

            ApplicationVersion applicationVersion = null;

            if (!string.IsNullOrWhiteSpace(ApplicationVersion))
            {
                applicationVersion = await context.FindApplicationVersionAsync(ApplicationVersion, cancellationToken);
            }

            //Set the application version.
            device.ApplicationVersionId = applicationVersion?.Id;

            await context.Client.Devices.UpdateDeviceAsync(device, cancellationToken);

            return 0;
        }
    }
}