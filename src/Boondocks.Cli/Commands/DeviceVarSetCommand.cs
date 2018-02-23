namespace Boondocks.Cli.Commands
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.Contracts;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("device-var-set", HelpText = "Sets an device environment variable.")]
    public class DeviceVarSetCommand : CommandBase
    {
        [Option('n', "name", Required = true, HelpText = "The name of the variable.")]
        public string Name { get; set; }

        [Option('v', "value", Required = true, HelpText = "The value of the variable.")]
        public string Value { get; set; }

        [Option('d', "device", Required = true, HelpText = "The device to update.")]
        public string Device { get; set; }

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
                var request = new CreateDeviceEnvironmentVariableRequest()
                {
                    DeviceId = device.Id,
                    Name = Name,
                    Value = Value
                };

                //Create the new one
                await context.Client.DeviceEnvironmentVariables.CreateEnvironmentVariableAsync(request,
                    cancellationToken);
            }
            else
            {
                //Create a new one
                variable.Value = Value;

                //Update the existing one
                await context.Client.DeviceEnvironmentVariables.UpdateEnvironmentVariable(variable, cancellationToken);
            }

            return 0;
        }
    }
}