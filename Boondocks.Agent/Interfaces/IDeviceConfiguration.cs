using System;

namespace Boondocks.Agent.Interfaces
{
    public interface IDeviceConfiguration
    {
        string DeviceApiUrl { get; }

        Guid DeviceId { get; }

        Guid DeviceKey { get; }

        string DockerEndpoint { get; }

        int PollSeconds { get; }
    }
}