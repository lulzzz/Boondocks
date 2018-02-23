namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.Contracts;

    [Verb("device-new", HelpText = "Create a new device.")]
    public class DeviceNewCommand : CommandBase
    {
        [Option('n', "name", Required = true, HelpText = "The name of the device.")]
        public string Name { get; set; }

        [Option('a', "app", Required = true, HelpText = "The name or id of the application.")]
        public string Application { get; set; }

        [Option('k', "dev-key", HelpText = "Specify a device key for this device.")]
        public string DeviceKey { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //Get the application
            var application = await context.FindApplicationAsync(Application, cancellationToken);

            if (application == null)
                return 1;

            var deviceKey = DeviceKey.TryParseGuid();

            var request = new CreateDeviceRequest
            {
                ApplicationId = application.Id,
                Name = Name,
                DeviceKey = deviceKey,
            };

            //Create the device
            var device = await context.Client.Devices.CreateDeviceAsync(request, cancellationToken);

            //Let the user know what happened.
            Console.WriteLine($"Device {device.Id} created with name '{device.Name}'.");

            return 0;
        }
    }
}