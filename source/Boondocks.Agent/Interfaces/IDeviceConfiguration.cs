namespace Boondocks.Agent.Interfaces
{
    using System;

    internal interface IDeviceConfiguration
    {
        string DeviceApiUrl { get; }

        Guid DeviceId { get; }

        Guid DeviceKey { get; }

        string DockerEndpoint { get; }

        int PollSeconds { get; }

        string RegistryEndpoint { get; }
    }
}