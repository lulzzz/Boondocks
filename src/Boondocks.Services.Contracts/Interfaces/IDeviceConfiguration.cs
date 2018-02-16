namespace Boondocks.Services.Contracts.Interfaces
{
    using System;

    public interface IDeviceConfiguration
    {
        string DeviceApiUrl { get; }

        Guid DeviceId { get; }

        Guid DeviceKey { get; }

        int PollSeconds { get; }
    }
}