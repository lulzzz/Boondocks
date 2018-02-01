namespace Boondocks.Services.Device.Contracts
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    ///     The response to a heartbeat request.
    /// </summary>
    public class HeartbeatResponse
    {
        /// <summary>
        ///     The configuration version of this device.
        /// </summary>
        [JsonProperty("configuration-version")]
        public Guid ConfigurationVersion { get; set; }
    }
}