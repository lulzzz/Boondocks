using Boondocks.Device.Api.Models;
using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;
using System;

namespace Boondocks.Device.Api.Commands
{
    /// <summary>
    /// Command dispatched when a heartbeat is received from a device.
    /// </summary>
    public class HeartbeatReceived : Command<DeviceVersion>
    {
        public Guid DeviceId { get; private set; }
        public DeviceHeartbeatModel Heartbeat { get; private set; }

        private HeartbeatReceived() { }

        public static HeartbeatReceived FromDevice(Guid deviceId, DeviceHeartbeatModel deviceHeartbeat)
        {
            if (deviceHeartbeat == null)
                throw new ArgumentNullException(nameof(deviceHeartbeat), 
                    "Command can't be created from null model.");

            return new HeartbeatReceived { 
                DeviceId = deviceId, 
                Heartbeat = deviceHeartbeat };
        }
    }
}