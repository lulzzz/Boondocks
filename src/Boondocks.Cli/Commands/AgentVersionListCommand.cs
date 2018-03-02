namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExtensionMethods;
    using Services.DataAccess.Domain;
    using Services.Management.WebApiClient;

    [Verb("agent-version-list", HelpText = "Lists the agent versions for a given architecture.")]
    public class AgentVersionListCommand : CommandBase
    {
        [Option('t', "device-type", Required = true, HelpText="The device type.")]
        public string DeviceType { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            DeviceType deviceType = await context.FindDeviceTypeAsync(DeviceType, cancellationToken);

            var request = new GetAgentVersionsRequest()
            {
                DeviceTypeId = deviceType?.Id
            };

            var versions = await context.Client.AgentVersions.GetAgentVersions(request, cancellationToken);

            versions.DisplayEntities(v => $"{v.Id}: {v.Name}");

            return 0;
        }
    }
}
