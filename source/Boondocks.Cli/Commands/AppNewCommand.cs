using System;
using System.Threading.Tasks;
using Boondocks.Base;
using Boondocks.Services.Contracts;
using Boondocks.Services.Management.Contracts;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("app-new", HelpText = "Create a new application.")]
    public class AppNewCommand : CommandBase
    {
        [Option('d', "device-type-id", Required = true, HelpText = "The device type id.")]
        public string DeviceTypeId { get; set; }

        [Option('n', "Name", Required = true, HelpText = "The name of the application.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            Guid? deviceTypeId = DeviceTypeId.TryParseGuid();

            if (deviceTypeId == null)
            {
                Console.WriteLine("Please specify a valid value for device-type-id.");
                return 1;
            }

            var request = new CreateApplicationRequest
            {
                DeviceTypeId = deviceTypeId.Value,
                Name = Name
            };

            //Create the application.
            Application application = await context.Client.Applications.CreateApplicationAsync(request);

            Console.WriteLine($"Application {application.Id} created.");

            return 0;
        }
    }
}