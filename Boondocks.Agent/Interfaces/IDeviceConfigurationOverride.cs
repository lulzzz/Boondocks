using System;

namespace Boondocks.Agent.Interfaces
{
    /// <summary>
    /// Intended to provide a means to override the device configuration with command line options.
    /// </summary>
    public interface IDeviceConfigurationOverride
    {
        string DeviceApiUrl { get; }

        Guid? DeviceId { get; }

        Guid? DeviceKey { get; }

        string DockerEndpoint { get; }

        int? PollSeconds { get; }
    }
}