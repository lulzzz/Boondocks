﻿namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;
    using Services.DataAccess.Domain;
    using Services.Management.Contracts;

    [Verb("app-new", HelpText = "Create a new application.")]
    public class AppNewCommand : CommandBase
    {
        [Option('d', "device-type", Required = true, HelpText = "The device type")]
        public string DeviceType { get; set; }

        [Option('n', "Name", Required = true, HelpText = "The name of the application.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //Get all of the device types
            DeviceType[] deviceTypes = await context.Client.DeviceTypes.GetDeviceTypesAsync(cancellationToken);

            //Find the device type
            DeviceType deviceType = deviceTypes.FindEntity(DeviceType);

            if (deviceType == null)
            {
                Console.WriteLine($"Unable to find device type '{DeviceType}'.");
                return 1;
            }

            //Create the request
            var request = new CreateApplicationRequest
            {
                DeviceTypeId = deviceType.Id,
                Name = Name
            };

            //Create the application.
            var application = await context.Client.Applications.CreateApplicationAsync(request, cancellationToken);

            Console.WriteLine($"Application {application.Id} created.");

            return 0;
        }
    }
}