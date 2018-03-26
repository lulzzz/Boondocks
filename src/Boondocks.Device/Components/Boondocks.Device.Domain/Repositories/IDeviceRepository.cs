using Boondocks.Device.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Boondocks.Device.Domain.Repositories
{
    /// <summary>
    /// Repository responsible for loading entity from persisted state.
    /// </summary>
    public interface IDeviceRepository
    {
        Task<DeviceEntity> GetDevice(Guid deviceId);

        /// <summary>
        /// Return the current device version for an identified device.
        /// </summary>
        /// <param name="deviceId">Value identifying the device.</param>
        /// <returns>Entity containing the device version information.</returns>
        Task<DeviceVersion> GetDeviceVersionInfo(Guid deviceId);

        Task<AgentVersion> GetAgentVersion(Guid id);

        /// <summary>
        /// Records the current device status.
        /// </summary>
        /// <param name="status">The current heartbeat status.</param>
        Task UpdateDeviceStatus(DeviceStatus status);

        /// <summary>
        /// Writes a new device log to persisted storage.
        /// </summary>
        /// <param name="log">The device log to write.</param>
        /// <param name="purgeBeforeWrite">If true, the existing application
        /// logs for the device will be deleted.  Otherwise, no action is taken.</param>
        /// <returns>Future completed task.</returns>
        Task WriteDeviceLog(DeviceLog log, bool purgeBeforeWrite = false);
    }
}