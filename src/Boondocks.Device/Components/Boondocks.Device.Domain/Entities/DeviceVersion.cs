using System;

namespace Boondocks.Device.Domain.Entities
{
    /// <summary>
    /// The response to a heartbeat request.
    /// </summary>
    public class DeviceVersion 
    {
        /// <summary>
        /// The configuration version of this device.
        /// </summary>
        public Guid ConfigurationVersion { get; set; }
    }
}