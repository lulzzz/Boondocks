namespace Boondocks.Device.Client.Models
{
    public class DeviceHeartbeatModel
    {
        public int UptimeSeconds { get; set; }
        public string AgentVersion { get; set; }
        public string ApplicationVersion { get; set; }
        public string RootFileSystemVersion { get; set; }
        public string State { get; set; }
    }
}