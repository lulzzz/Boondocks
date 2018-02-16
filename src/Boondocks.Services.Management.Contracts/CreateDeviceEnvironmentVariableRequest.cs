namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateDeviceEnvironmentVariableRequest : CreateEnvironmentVariableRequest
    {
        /// <summary>
        ///     Get the device
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }
    }
}