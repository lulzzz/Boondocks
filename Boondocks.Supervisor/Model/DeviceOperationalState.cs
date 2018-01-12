using System;
using Boondocks.Services.Device.Contracts;

namespace Boondocks.Supervisor.Model
{
    /// <summary>
    /// The operation state of the device.
    /// </summary>
    public class DeviceOperationalState
    {
        /// <summary>
        /// The application version.
        /// </summary>
        public ApplicationReference CurrentApplicationVersion { get; set; }

        /// <summary>
        /// Keep track of this to see if it has changed.
        /// </summary>
        public Guid? ConfigurationVersion { get; set; }
    }
}