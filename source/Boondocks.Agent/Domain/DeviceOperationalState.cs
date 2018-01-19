using System;
using System.Collections.Generic;
using Boondocks.Services.Device.Contracts;

namespace Boondocks.Agent.Domain
{
    /// <summary>
    /// The operation state of the device.
    /// </summary>
    internal class DeviceOperationalState
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
        /// This will be used to calculate a delta.
        /// </summary>
        public VersionReference PreviousApplicationVersion { get; set; }

        /// <summary>
        /// Old applications that we need to remove.
        /// </summary>
        public List<VersionReference> ApplicationsToRemove { get; set; }

        /// <summary>
        /// Keep track of this to see if it has changed.
        /// </summary>
        public Guid? ConfigurationVersion { get; set; }
    }
}