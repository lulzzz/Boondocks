namespace Boondocks.Cli.Commands
{
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("device-type-list", HelpText = "List device types.")]
    public class DeviceTypeListCommand : CommandBase
    {
        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            var deviceTypes = await context.Client.DeviceTypes.GetDeviceTypesAsync();

            deviceTypes.DisplayEntities(d => $"{d.Id}: {d.Name}");

            return 0;
        }
    }
}