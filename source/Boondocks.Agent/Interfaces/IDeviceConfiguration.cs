using System;

namespace Boondocks.Agent.Interfaces
{
    internal interface IDeviceConfiguration
    {
        string DeviceApiUrl { get; }

        Guid DeviceId { get; }

        Guid DeviceKey { get; }

        string DockerEndpoint { get; }

        int PollSeconds { get; }

        string DockerRepositoryUrl { get; }
    }
}