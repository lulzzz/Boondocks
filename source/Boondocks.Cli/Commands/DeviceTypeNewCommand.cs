using System;
using System.Threading.Tasks;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("device-type-new", HelpText = "Create a new device type.")]
    public class DeviceTypeNewCommand : CommandBase
    {
        [Option('n', "Name", Required = true, HelpText = "The name of the device type to create.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            var deviceType = await context.Client.CreateDeviceTypeAsync(Name);

            Console.WriteLine($"DeviceType {deviceType.Id} created.");

            return 0;
        }
    }
}