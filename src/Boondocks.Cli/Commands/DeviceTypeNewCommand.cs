﻿namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Services.Management.Contracts;

    [Verb("device-type-new", HelpText = "Create a new device type.")]
    public class DeviceTypeNewCommand : CommandBase
    {
        [Option('n', "Name", Required = true, HelpText = "The name of the device type to create.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var request = new CreateDeviceTypeRequest
            {
                Name = Name
            };

            var deviceType = await context.Client.DeviceTypes.CreateDeviceTypeAsync(request, cancellationToken);

            Console.WriteLine($"DeviceType {deviceType.Id} created.");

            return 0;
        }
    }
}