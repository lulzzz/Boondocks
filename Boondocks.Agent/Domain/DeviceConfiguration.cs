﻿using System;
using Boondocks.Agent.Interfaces;
using Newtonsoft.Json;

namespace Boondocks.Agent.Domain
{
    /// <summary>
    /// The configuration for a device.
    /// </summary>
    internal class DeviceConfiguration : IDeviceConfiguration
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
        /// The address of the docker management bits on the device.
        /// </summary>
        [JsonProperty("dockerEndpoint")]
        public string DockerEndpoint { get; set; }

        /// <summary>
        /// The number of seconds to wait between polling the server for new versions.
        /// </summary>
        [JsonProperty("pollSeconds")]
        public int PollSeconds { get; set; }
    }
}