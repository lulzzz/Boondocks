﻿namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("device-set-app-version", HelpText = "Sets the current application version for a given device.")]
    public class DeviceSetAppVersion : CommandBase
    {
        [Option('d', "device-id", Required = true, HelpText = "The device id to update.")]
        public string DeviceId { get; set; }

        [Option('v', "app-version-id", Required = true, HelpText = "The id of the application version to use.")]
        public string ApplicationVersionId { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            var deviceId = DeviceId.TryParseGuid();
            var applicationVersionId = ApplicationVersionId.TryParseGuid();

            if (deviceId == null)
            {
                Console.WriteLine("Invalid device-id.");
                return 1;
            }

            if (applicationVersionId == null)
            {
                Console.WriteLine("Invalid app-version-id.");
                return 1;
            }

            var device = await context.Client.Devices.GetDeviceAsync(deviceId.Value, cancellationToken);

            device.ApplicationVersionId = applicationVersionId;

            await context.Client.Devices.UpdateDeviceAsync(device, cancellationToken);

            return 0;
        }
    }
}