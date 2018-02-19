namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using CommandLine;
    using ExtensionMethods;
    using Services.Management.WebApiClient;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("agent-version-list", HelpText = "Lists the agent versions for a given architecture.")]
    public class AgentVersionListCommand : CommandBase
    {
        [Option('a', "arch", Required = true, HelpText="The device architecture.")]
        public string DeviceArchitecture { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            Guid? deviceArchitectureId = DeviceArchitecture.TryParseGuid(false);

            var request = new GetAgentVersionsRequest();

            if (deviceArchitectureId == null)
            {
                var deviceArchitecture = await context.FindDeviceArchitecture(DeviceArchitecture, cancellationToken);

                if (deviceArchitecture == null)
                {
                    return 1;
                }

                request.DeviceArchitectureId = deviceArchitecture.Id;
            }
            else
            {
                request.DeviceArchitectureId = deviceArchitectureId.Value;
            }

            var versions = await context.Client.AgentVersions.GetAgentVersions(request, cancellationToken);

            versions.DisplayEntities(v => $"{v.Id}: {v.Name}");

            return 0;
        }
    }
}
