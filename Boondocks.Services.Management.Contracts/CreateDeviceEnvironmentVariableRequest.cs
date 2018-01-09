using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateDeviceEnvironmentVariableRequest : CreateEnvironmentVariableRequest
    {
        /// <summary>
        /// Get the device
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }
    }
}