using System;
using System.Threading.Tasks;
using Boondocks.Base;
using Boondocks.Services.Contracts;
using Boondocks.Services.Management.Contracts;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("device-new", HelpText = "Create a new device.")]
    public class DeviceNewCommand : CommandBase
    {
        [Option('n', "name", Required = true, HelpText = "The name of the device.")]
        public string Name { get; set; }

        [Option('a', "app-id", Required = true, HelpText = "The id of the application to create this device in.")]
        public string ApplicationId { get; set; }

        [Option('v', "app-ver", HelpText = "The initial application version to use.")]
        public string ApplicationVersionId { get; set; }

        [Option('k', "dev-key", HelpText = "Specify a device key for this device.")]
        public string DeviceKey { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            Guid? applicationId = ApplicationId.TryParseGuid();
            Guid? deviceKey = DeviceKey.TryParseGuid();
            Guid? applicationVersionId = ApplicationVersionId.TryParseGuid();

            if (applicationId == null)
            {
                Console.WriteLine("Invalid format for ApplicationId.");
                return 1;
            }

            var request = new CreateDeviceRequest
            {
                ApplicationId = applicationId.Value,
                Name = Name,
                DeviceKey = deviceKey
            };

            //Create the device
            Device device = await context.Client.Devices.CreateDeviceAsync(request);

            //Let the user know what happened.
            Console.WriteLine($"Device {device.Id} created with name '{device.Name}'.");

            return 0;
        }
    }
}