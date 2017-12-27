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
        [JsonProperty("uptime-seconds")]
        public double UpTimeSeconds { get; set; }

        /// <summary>
        /// The version of the supervisor.
        /// </summary>
        [JsonProperty("supervisor-version")]
        public string SupervisorVersion { get; set; }

        /// <summary>
        /// The version of the root file system.
        /// </summary>
        [JsonProperty("root-file-system-version")]
        public string RootFileSystemVersion { get; set; }

        /// <summary>
        /// The application version.
        /// </summary>
        [JsonProperty("application-version")]
        public string ApplicationVersion { get; set; }
    }
}