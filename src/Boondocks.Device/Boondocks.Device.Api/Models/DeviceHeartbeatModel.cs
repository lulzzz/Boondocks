using Boondocks.Device.Domain.Entities;

namespace Boondocks.Device.Api.Models
{
    public class DeviceHeartbeatModel
    {
        public int UptimeSeconds { get; set; }
        public string AgentVersion { get; set; }
        public string ApplicationVersion { get; set; }
        public string RootFileSystemVersion { get; set; }
        public DeviceState State { get; set; }
    }
}