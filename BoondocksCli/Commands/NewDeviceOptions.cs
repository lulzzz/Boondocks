using System;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using Boondocks.Services.Contracts;
using CommandLine;

namespace BoondocksCli.Commands
{
    [Verb("new-dev", HelpText = "Create a new device.")]
    public class NewDeviceOptions : OptionsBase
    {
        [Option('n', "name", Required = true, HelpText = "The name of the device.")]
        public string Name { get; set; }

        [Option('a', "app-id", Required = true, HelpText = "The id of the application to create this device in.")]
        public string ApplicationId { get; set; }

        [Option('v', "app-ver", HelpText = "The initial application version to use.")]
        public string ApplicationVersionId { get; set; }

        [Option('k', "dev-key", HelpText = "Specify a device key for this device.")]
        public string DeviceKey { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            Guid? applicationId = ApplicationId.ParseGuid();
            Guid? deviceKey = DeviceKey.ParseGuid();
            Guid? applicationVersionId = ApplicationVersionId.ParseGuid();

            if (applicationId == null)
            {
                Console.WriteLine("Invalid format for ApplicationId.");
                return 1;
            }

            //Create the device
            Device device = await context.Client.CreateDeviceAsync(applicationId.Value, Name, applicationVersionId, deviceKey);

            //Let the user know what happened.
            Console.WriteLine($"Device {device.Id} created with name '{device.Name}'.");

            return 0;
        }
    }
}