using System;
using Boondocks.Services.Contracts;
using Newtonsoft.Json;

namespace Boondocks.Services.Device.Contracts
{
    /// <summary>
    /// A Ping request.
    /// </summary>
    public class HeartbeatRequest
    {
        /// <summary>
        /// The number of seconds that the supervisor has been up for.
        /// </summary>
        [JsonProperty("uptimeSeconds")]
        public double UptimeSeconds { get; set; }

        /// <summary>
        /// The version of the supervisor.
        /// </summary>
        [JsonProperty("supervisorVversionId")]
        public Guid SupervisorVersionId { get; set; }

        /// <summary>
        /// The version of the root file system.
        /// </summary>
        [JsonProperty("rootFileSystemVersionId")]
        public Guid RootFileSystemVersionId { get; set; }

        /// <summary>
        /// The application version.
        /// </summary>
        [JsonProperty("applicationVersionId")]
        public Guid ApplicationVersionId { get; set; }

        /// <summary>
        /// The current state of the devices.
        /// </summary>
        [JsonProperty("state")]
        public DeviceState State { get; set; }
    }
}