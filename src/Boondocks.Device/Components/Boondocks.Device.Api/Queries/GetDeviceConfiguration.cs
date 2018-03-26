using System;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Domain.Entities;
using NetFusion.Messaging.Types;

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