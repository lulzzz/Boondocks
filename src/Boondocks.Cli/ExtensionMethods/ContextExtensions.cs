﻿namespace Boondocks.Cli.ExtensionMethods
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

        public static async Task<AgentVersion> FindAgentVersion(this CommandContext context,
            Guid deviceTypeId,
            string search, CancellationToken cancellationToken)
        {
            //Get the applications
            var entities = await context.Client.AgentVersions.GetAgentVersions(new GetAgentVersionsRequest()
            {
                DeviceTypeId = deviceTypeId
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

        public static async Task<DeviceType> FindDeviceTypeAsync(this CommandContext context, string search,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(search))
                return null;

            Guid? entityId = search.TryParseGuid(false);

            DeviceType entity;

            if (entityId == null)
            {
                var entities = await context.Client.DeviceTypes.GetDeviceTypesAsync(cancellationToken);

                entity = entities.FindEntity(search);
            }
            else
            {
                entity = await context.Client.DeviceTypes.GetDeviceTypeAsync(entityId.Value, cancellationToken);
            }

            if (entity == null)
            {
                Console.Error.WriteLine($"Unable to find device type '{search}'.");
            }

            return entity;
        }
    }
 }