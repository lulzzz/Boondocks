namespace Boondocks.Services.Contracts
{
    using System;
    using Interfaces;
    using Newtonsoft.Json;

    /// <summary>
    ///     The configuration for a device.
    /// </summary>
    public class DeviceConfiguration : IDeviceConfiguration
    {
        /// <summary>
        /// The unique ID of this device.
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// The key for this device.
        /// </summary>
        [JsonProperty("deviceKey")]
        public Guid DeviceKey { get; set; }

        /// <summary>
        /// The api of the device
        /// </summary>
        [JsonProperty("deviceApiUrl")]
        public string DeviceApiUrl { get; set; }

        /// <summary>
        /// The number of seconds to wait between polling the server for new versions.
        /// </summary>
        [JsonProperty("pollSeconds")]
        public int PollSeconds { get; set; }

    }
}