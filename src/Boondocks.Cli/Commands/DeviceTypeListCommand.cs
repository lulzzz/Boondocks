namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("device-type-list", HelpText = "List device types.")]
    public class DeviceTypeListCommand : CommandBase
    {
        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var deviceTypes = await context.Client.DeviceTypes.GetDeviceTypesAsync(cancellationToken);

            deviceTypes.DisplayEntities(d => $"{d.Id}: {d.Name}");

            return 0;
        }
    }
}