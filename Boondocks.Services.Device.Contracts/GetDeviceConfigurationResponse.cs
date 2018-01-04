using Newtonsoft.Json;

namespace Boondocks.Services.Device.Contracts
{
    public class GetDeviceConfigurationResponse
    {
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

        /// <summary>
        /// The effective environment variables for this device.
        /// </summary>
        [JsonProperty("environment-variables")]
        public EnvironmentVariable[] EnvironmentVariables { get; set; }
    }
}