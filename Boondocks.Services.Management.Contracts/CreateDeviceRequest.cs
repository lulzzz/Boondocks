using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateDeviceRequest
    {
        /// <summary>
        /// The application to create the device in.
        /// </summary>
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// The name of the device.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        [JsonProperty("deviceKey")]
        public Guid? DeviceKey { get; set; }
    }

    public class CreateDeviceEnvironmentVariableRequest
    {
        /// <summary>
        /// Get the device
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// The name of the variable.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}