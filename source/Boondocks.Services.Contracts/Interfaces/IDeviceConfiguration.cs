namespace Boondocks.Services.Contracts.Interfaces
{
    using System;

    public interface IDeviceConfiguration
    {
        string DeviceApiUrl { get; }

        Guid DeviceId { get; }

        Guid DeviceKey { get; }

        string DockerEndpoint { get; }

        int PollSeconds { get; }

        string RegistryHost { get; }
    }
}