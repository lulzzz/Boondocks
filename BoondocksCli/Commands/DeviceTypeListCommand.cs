using System;
using System.Threading.Tasks;
using BoondocksCli.ExtensionMethods;
using CommandLine;

namespace BoondocksCli.Commands
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