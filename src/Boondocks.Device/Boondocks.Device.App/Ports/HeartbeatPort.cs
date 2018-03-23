using System;
using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.Api.Commands;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NetFusion.Messaging;

namespace Boondocks.Device.App.Ports
{
    /// <summary>
    /// Port responsible for handeling a command dispatched when a
    /// heartbeat is received from a device.
    /// </summary>
    public class HeartbeatPort : IMessageConsumer
    {
        private readonly ILogger<HeartbeatPort> _logger;
        private readonly IRepositoryContext<DeviceDb> _repoContext;
        private readonly IDeviceRepository _deviceRepo;

        public HeartbeatPort(
            ILogger<HeartbeatPort> logger,
            IRepositoryContext<DeviceDb> repoContext,
            IDeviceRepository deviceRepo)
        {
            _logger = logger;
            _repoContext = repoContext;
            _deviceRepo = deviceRepo;
        }

        [InProcessHandler]
        public async Task<DeviceVersion> When (HeartbeatReceived command)
        {
            var heartbeat = command.Heartbeat;

            // Create domain model for the device associated with the current context and
            // record the received heartbeat information.
            var deviceStatus = DeviceStatus.ForExistingDevice(command.DeviceId, 
                status => status.RecordHeartbeat(
                    heartbeat.State, 
                    heartbeat.UptimeSeconds,
                    heartbeat.AgentVersion,
                    heartbeat.ApplicationVersion, 
                    heartbeat.RootFileSystemVersion));

            DeviceVersion deviceVersion = null;
            using(_repoContext) 
            {
                // Update the device status and return it's current configuration version.
                await _deviceRepo.UpdateDeviceStatus(deviceStatus);
                deviceVersion = await _deviceRepo.GetDeviceVersionInfo(deviceStatus.DeviceId);
            }

            _logger.LogTrace(
                "Heartbeat received for device {DeviceId}. Device has been up for {UptimeSeconds} " + 
                "seconds and is using ConfigurationVersion {ConfigurationVersion} sent.",
                deviceStatus.DeviceId, deviceStatus.UptimeSeconds, deviceVersion.ConfigurationVersion);

            return deviceVersion;
        }
    }
}