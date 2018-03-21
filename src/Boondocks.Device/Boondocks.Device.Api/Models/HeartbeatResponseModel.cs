using System;
using Boondocks.Device.Domain;

namespace Boondocks.Device.Api.Models
{
    /// <summary>
    /// The response to a heartbeat request.
    /// </summary>
    public class HeartbeatResponseModel 
    {
        /// <summary>
        /// The configuration version of this device.
        /// </summary>
        public Guid ConfigurationVersion { get; set; }
    }
}