namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Services.DataAccess.Domain;
    using Services.Management.Contracts;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("device-arch-new", HelpText = "Creates a new device architecture")]
    public class DeviceArchNewCommand : CommandBase
    {
        [Option('n', "name", Required = true, HelpText = "The name of the architecture.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            var request = new CreateDeviceArchitectureRequest
            {
                Name = Name
            };

            DeviceArchitecture deviceArchitecture = await context.Client.DeviceArchitectures.CreateDeviceArchitecture(request, cancellationToken);

            Console.WriteLine($"{deviceArchitecture.Id}");

            return 0;
        }
    }
}