using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;
using System;

namespace Boondocks.Device.Api.Queries
{
    /// <summary>
    /// Queries the device configuration for a specific device.
    /// </summary>
    public class GetDeviceConfiguration : Query<DeviceConfiguration>
    {
        public Guid DeviceId { get; }

        public GetDeviceConfiguration(Guid deviceId)
        {
            if (deviceId == null || deviceId == Guid.Empty)
            {
                throw new ArgumentException("Device Id not specified.", nameof(deviceId));
            }

            DeviceId =  deviceId;
        }
    }
}