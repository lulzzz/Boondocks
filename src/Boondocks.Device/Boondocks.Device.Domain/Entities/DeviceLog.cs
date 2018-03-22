using System;
using System.Collections.Generic;

namespace Boondocks.Device.Domain.Entities
{
    /// <summary>
    /// Domain entity representing a device log event.
    /// </summary>
    public class DeviceLog
    {
        private List<ApplicationLog> _logEntries = new List<ApplicationLog>();

        public Guid DeviceId { get; private set; }
        public IReadOnlyCollection<ApplicationLog> Entries { get; }

        private DeviceLog()
        {
            Entries = _logEntries;
        }

        // Creates a new log for an existing device.
        public static DeviceLog ForExistingDevice(Guid deviceId) {
            
            if (deviceId == Guid.Empty)
                throw new ArgumentException("Device Id not specified.", nameof(deviceId));
            
            return new DeviceLog {
                DeviceId = deviceId
            };
        }

        public void AddLog(ApplicationLog log)
        {
            log.Id = Guid.NewGuid();
            log.DeviceId = DeviceId;
            log.Validate();

            _logEntries.Add(log);
        }
    }
}