using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Device.Contracts
{
    public class GetDeviceConfigurationResponse
    {
        /// <summary>
        /// The version of the supervisor.
        /// </summary>
        [JsonProperty("supervisorVersion")]
        public VersionReference SupervisorVersion { get; set; }

        /// <summary>
        /// The version of the root file system.
        /// </summary>
        [JsonProperty("rootFileSystemVersionId")]
        public Guid? RootFileSystemVersionId { get; set; }

        /// <summary>
        /// The application version.
        /// </summary>
        [JsonProperty("applicationVersion")]
        public VersionReference ApplicationVersion { get; set; }

        /// <summary>
        /// The effective environment variables for this device.
        /// </summary>
        [JsonProperty("environmentVariables")]
        public EnvironmentVariable[] EnvironmentVariables { get; set; }

        /// <summary>
        /// The configuration version for this device.
        /// </summary>
        [JsonProperty("configurationVersion")]
        public Guid ConfigurationVersion { get; set; }
    }
}