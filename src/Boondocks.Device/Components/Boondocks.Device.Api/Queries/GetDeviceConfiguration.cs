using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;
using System;

namespace Boondocks.Device.Api.Queries
{
    public class GetDeviceConfiguration : Query<DeviceConfiguration>
    {
        public Guid DeviceId { get; }

        public GetDeviceConfiguration(Guid deviceId)
        {
            DeviceId =  deviceId;
        }
    }
}