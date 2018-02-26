namespace Boondocks.Cli.ExtensionMethods
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Base;
    using Services.Contracts;
    using Services.DataAccess.Domain;
    using Services.Management.WebApiClient;

    internal static  class ContextExtensions
    {
        public static async Task<Application> FindApplicationAsync(this CommandContext context, string search, CancellationToken cancellationToken)
        {
            //Get the applications
            var entities = await context.Client.Applications.GetApplicationsAsync(new GetApplicationsRequest(), cancellationToken);

            //Find the application
            var entity = entities.FindEntity(search);

            if (entity == null)
            {
                Console.WriteLine($"Unable to find application '{search}'.");
            }

            return entity;
        }

        public static async Task<DeviceArchitecture> FindDeviceArchitecture(this CommandContext context,
            string search, CancellationToken cancellationToken)
        {
            //Get the applications
            var entities = await context.Client.DeviceArchitectures.GetDeviceArchitectures(cancellationToken);

            //Find the application
            var entity = entities.FindEntity(search);

            if (entity == null)
            {
                Console.WriteLine($"Unable to find device architecture '{search}'.");
            }

            return entity;
        }

        public static async Task<AgentVersion> FindAgentVersion(this CommandContext context,
            Guid deviceArchitectureId,
            string search, CancellationToken cancellationToken)
        {
            //Get the applications
            var entities = await context.Client.AgentVersions.GetAgentVersions(new GetAgentVersionsRequest()
            {
                DeviceArchitectureId = deviceArchitectureId
            } ,cancellationToken);

            //Find the application
            var entity = entities.FindEntity(search);

            if (entity == null)
            {
                Console.WriteLine($"Unable to find agent version '{search}'.");
            }

            return entity;
        }

        public static async Task<ApplicationVersion> FindApplicationVersionAsync(this CommandContext context, string search,  CancellationToken cancellationToken)
        {
            //Get the applications
            var entities = await context.Client.ApplicationVersions.GetApplicationVersionsAsync(new GetApplicationVersionsRequest(), cancellationToken);

            //Find the application
            var entity = entities.FindEntity(search);

            if (entity == null)
            {
                Console.WriteLine($"Unable to find application version '{search}'.");
            }

            return entity;
        }

        public static async Task<Device> FindDeviceAsync(this CommandContext context, string search, CancellationToken cancellationToken)
        {
            Guid? deviceId = search.TryParseGuid(false);

            Device device;

            if (deviceId == null)
            {
                //Get the applications
                var devices = await context.Client.Devices.GetDevicesAsync(new GetDevicesRequest(), cancellationToken);

                //Find the application
                device = devices.FindEntity(search);
            }
            else
            {
                //Get the device using its id
                device = await context.Client.Devices.GetDeviceAsync(deviceId.Value, cancellationToken);
            }

            if (device == null)
            {
                Console.WriteLine($"Unable to find device '{search}'.");
            }

            return device;
        }
    }
 }