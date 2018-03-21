using System;
using System.Threading.Tasks;
using Boondocks.Device.Domain.Entities;

namespace Boondocks.Device.Domain.Repositories
{
    /// <summary>
    /// Repository responsible for loading entity from persisted state.
    /// </summary>
    public interface IDeviceRepository
    {
        /// <summary>
        /// Return the current device version for an identified device.
        /// </summary>
        /// <param name="deviceId">Value identifying the device.</param>
        /// <returns>Entity containing the device version information.</returns>
        Task<DeviceVersion> GetDeviceVersionInfo(Guid deviceId);

        /// <summary>
        /// Records the current device status.
        /// </summary>
        /// <param name="status">The current heartbeat status.</param>
        Task UpdateDeviceStatus(DeviceStatus status);
    }
}