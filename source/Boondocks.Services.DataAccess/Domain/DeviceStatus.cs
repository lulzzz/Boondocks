using System;

namespace Boondocks.Services.DataAccess.Domain
{
    public class DeviceStatus
    {
        public Guid Id { get; set; }

        public Guid? DeviceRootFileSystem { get; set; }

        public Guid? DeviceApplicationVersionId { get; set; }

        public Guid? DeviceSupervisorVersionId { get; set; }

        public string LocalIpAddress { get; set; }

        public DeviceState State { get; set; }

        public DateTime? LastContactUtc { get; set; }
    }
}