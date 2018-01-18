using System;
using Boondocks.Agent.Interfaces;

namespace Boondocks.Agent.Model
{
    public class DeviceConfiguration : IDeviceConfiguration
    {
        /// <summary>
        /// The unique ID of this device.
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// The key for this device.
        /// </summary>
        public Guid DeviceKey { get; set; }

        /// <summary>
        /// The api of the device 
        /// </summary>
        public string DeviceApiUrl { get; set; }

        /// <summary>
        /// The address of the docker management bits on the device.
        /// </summary>
        public string DockerEndpoint { get; set; }

        /// <summary>
        /// The number of seconds to wait between polling the server for new versions.
        /// </summary>
        public int PollSeconds { get; set; }
    }
}