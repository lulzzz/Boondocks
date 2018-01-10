using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Device.Contracts
{
    public class GetDeviceConfigurationResponse
    {
        /// <summary>
        /// The version of the supervisor.
        /// </summary>
        [JsonProperty("supervisorVersionId")]
        public Guid? SupervisorVersionId { get; set; }

        /// <summary>
        /// The version of the root file system.
        /// </summary>
        [JsonProperty("rootFileSystemVersionId")]
        public Guid? RootFileSystemVersionId { get; set; }

        /// <summary>
        /// The application version.
        /// </summary>
        [JsonProperty("applicationVersionId")]
        public Guid? ApplicationVersionId { get; set; }

        /// <summary>
        /// The effective environment variables for this device.
        /// </summary>
        [JsonProperty("environment-variables")]
        public EnvironmentVariable[] EnvironmentVariables { get; set; }
    }
}