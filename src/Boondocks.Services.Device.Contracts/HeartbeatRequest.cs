namespace Boondocks.Services.Device.Contracts
{
    using DataAccess.Domain;
    using Newtonsoft.Json;

    /// <summary>
    /// A Ping request.
    /// </summary>
    public class HeartbeatRequest
    {
        /// <summary>
        /// The number of seconds that the agent has been up for.
        /// </summary>
        [JsonProperty("uptimeSeconds")]
        public double UptimeSeconds { get; set; }

        /// <summary>
        /// The version of the agent.
        /// </summary>
        [JsonProperty("agentVersion")]
        public string AgentVersion { get; set; }

        /// <summary>
        /// The application version.
        /// </summary>
        [JsonProperty("applicationVersion")]
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// The version of the root file system.
        /// </summary>
        [JsonProperty("rootFileSystemVersion")]
        public string RootFileSystemVersion { get; set; }

        /// <summary>
        /// The current state of the devices.
        /// </summary>
        [JsonProperty("state")]
        public DeviceState State { get; set; }
    }
}