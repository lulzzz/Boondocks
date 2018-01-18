using System.Threading.Tasks;
using Boondocks.Cli.ExtensionMethods;
using CommandLine;

namespace Boondocks.Cli.Commands
{
    [Verb("device-type-list", HelpText = "List device types.")]
    public class DeviceTypeListCommand : CommandBase
    {
        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            var deviceTypes = await context.Client.GetDeviceTypes();

            deviceTypes.DisplayEntities(d => $"{d.Id}: {d.Name}");

            return 0;
        }
    }
}