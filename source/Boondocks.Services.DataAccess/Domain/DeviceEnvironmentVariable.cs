using System;

namespace Boondocks.Services.DataAccess.Domain
{
    public class DeviceEnvironmentVariable : EnvironmentVariable
    {
        public Guid DeviceId { get; set; }
    }
}