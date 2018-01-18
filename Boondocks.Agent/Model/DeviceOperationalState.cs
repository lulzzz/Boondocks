using System;
using Boondocks.Services.Device.Contracts;

namespace Boondocks.Agent.Model
{
    /// <summary>
    /// The operation state of the device.
    /// </summary>
    public class DeviceOperationalState
    {
        /// <summary>
        /// The current application. This is what we should make sure is running before working about the NextApplicationVersion.
        /// </summary>
        public VersionReference CurrentApplicationVersion { get; set; }

        /// <summary>
        /// This is the application that we should be downloading / installing.
        /// </summary>
        public VersionReference NextApplicationVersion { get; set; }

        /// <summary>
        /// Keep track of this to see if it has changed.
        /// </summary>
        public Guid? ConfigurationVersion { get; set; }
    }
}