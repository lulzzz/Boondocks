namespace Boondocks.Agent.Domain
{
    using System;
    using Services.Device.Contracts;

    /// <summary>
    /// The operational state of the device.
    /// </summary>
    internal class DeviceOperationalState
    {
        /// <summary>
        /// The current application. This is what we should make sure is running before working about the
        /// NextApplicationVersion.
        /// </summary>
        public VersionReference CurrentApplicationVersion { get; set; }

        /// <summary>
        /// This is the application that we should be downloading / installing.
        /// </summary>
        public VersionReference NextApplicationVersion { get; set; }

        /// <summary>
        /// The current supervisor version.
        /// </summary>
        public VersionReference CurrentSupervisorVersion { get; set; }

        /// <summary>
        /// The next supervisor version.
        /// </summary>
        public VersionReference NextSupervisorVersion { get; set; }

        /// <summary>
        /// Keep track of this to see if it has changed.
        /// </summary>
        public Guid? ConfigurationVersion { get; set; }
    }
}