namespace Boondocks.Agent.Domain
{
    using System;
    using Interfaces;

    public class DeviceConfigurationOverride : IDeviceConfigurationOverride
    {
        public string DeviceApiUrl { get; set; }

        public Guid? DeviceId { get; set; }

        public Guid? DeviceKey { get; set; }

        public string DockerEndpoint { get; set; }

        public int? PollSeconds { get; set; }
    }
}