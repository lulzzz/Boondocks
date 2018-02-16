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

    [Verb("supervisor-version-list", HelpText = "Lists the supervisor versions for a given architecture.")]
    public class SupervisorVersionListCommand : CommandBase
    {
        [Option('a', "arch", Required = true, HelpText="The device architecture.")]
        public string DeviceArchitecture { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
        {
            Guid? deviceArchitectureId = DeviceArchitecture.TryParseGuid(false);

            var request = new GetSupervisorVersionsRequest();

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

            var versions = await context.Client.SupervisorVersions.GetSupervisorVersions(request, cancellationToken);

            versions.DisplayEntities(v => $"{v.Id}: {v.Name}");

            return 0;
        }
    }
}
