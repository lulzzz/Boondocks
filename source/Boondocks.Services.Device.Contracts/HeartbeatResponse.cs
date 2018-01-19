using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Boondocks.Services.Device.Contracts
{
    /// <summary>
    /// The response to a heartbeat request.
    /// </summary>
    public class HeartbeatResponse
    {
        /// <summary>
        /// The configuration version of this device.
        /// </summary>
        [JsonProperty("configuration-version")]
        public Guid ConfigurationVersion { get; set; }

      
    }
}