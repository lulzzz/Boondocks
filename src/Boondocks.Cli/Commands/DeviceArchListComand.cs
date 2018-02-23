namespace Boondocks.Cli.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using ExtensionMethods;

    [Verb("device-arch-list", HelpText = "Lists device architectures")]
    public class DeviceArchListComand : CommandBase
    {
        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var devicesArhictectures = await context.Client.DeviceArchitectures.GetDeviceArchitectures(cancellationToken);

            devicesArhictectures.DisplayEntities(d => $"{d.Id:D}: {d.Name}");

            return 0;
        }
    }
}