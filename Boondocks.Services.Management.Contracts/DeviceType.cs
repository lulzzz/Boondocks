using System;

namespace Boondocks.Services.Management.Contracts
{
    public class DeviceType
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}