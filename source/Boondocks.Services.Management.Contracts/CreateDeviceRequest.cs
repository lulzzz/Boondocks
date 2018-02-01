namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateDeviceRequest
    {
        /// <summary>
        ///     The application to create the device in.
        /// </summary>
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }

        /// <summary>
        ///     The name of the device.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Optional
        /// </summary>
        [JsonProperty("deviceKey")]
        public Guid? DeviceKey { get; set; }
    }
}