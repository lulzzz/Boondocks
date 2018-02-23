namespace Boondocks.Cli.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("device-var-delete", HelpText = "Delete an application environment variable.")]
    public class DeviceVarDeleteCommand : CommandBase
    {
        [Option('d', "device", Required = true, HelpText = "The device to update.")]
        public string Device { get; set; }


        [Option('n', "name", Required = true, HelpText = "The name of the variable.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            //Get the device
            var device = await context.FindDeviceAsync(Device, cancellationToken);

            if (device == null)
            {
                return 1;
            }

            //Get the existing variables
            var variables = await context.Client.DeviceEnvironmentVariables.GetEnvironmentVariables(device.Id, cancellationToken);

            var variable = variables.FirstOrDefault(v => v.Name == Name);

            if (variable == null)
            {
                Console.WriteLine($"Unable to find variable '{Name}' for device '{device.Name}'.");
                return 1;
            }
            else
            {
                //Update the existing one
                await context.Client.DeviceEnvironmentVariables.DeleteEnvironmentVariable(variable.Id, cancellationToken);
            }

            return 0;
        }
    }
}