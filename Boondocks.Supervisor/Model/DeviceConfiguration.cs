using System;

namespace Boondocks.Supervisor.Model
{
    public class DeviceConfiguration
    {
        public string DeviceId { get; set; }

        public string DeviceKey { get; set; }

        public string DeviceApiUrl { get; set; }

        public string DockerEndpoint { get; set; }

        public int PollSeconds { get; set; }
    }
}