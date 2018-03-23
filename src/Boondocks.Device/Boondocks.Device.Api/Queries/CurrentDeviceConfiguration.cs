using System;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;

namespace Boondocks.Device.Api.Queries
{
    public class CurrentDeviceConfiguration : Query<DeviceConfiguration>
    {
        public Guid DeviceId { get; }

        public CurrentDeviceConfiguration(Guid deviceId)
        {
            DeviceId =  deviceId;
        }
    }
}