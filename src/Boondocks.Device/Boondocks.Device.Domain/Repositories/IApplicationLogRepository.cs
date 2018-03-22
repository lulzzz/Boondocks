using System;
using System.Threading.Tasks;
using Boondocks.Device.Domain.Entities;

namespace Boondocks.Device.Domain.Repositories
{
    /// <summary>
    /// Repository for managing device application logs.
    /// </summary>
    public interface IApplicationLogRepository
    {
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