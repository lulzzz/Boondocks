using System;

namespace Boondocks.Device.Domain.Entities
{
    /// <summary>
    /// Domain entity modeling the current or updated status 
    /// of a device.
    /// </summary>
    public class DeviceStatus
    {
        public Guid DeviceId { get; private set; }
        public DeviceState State { get; private set; }
        public int? UptimeSeconds { get; private set; }
        public string RootFileSystemVersion { get; private set; }
        public string ApplicationVersion { get; private set; }
        public string AgentVersion { get; private set; }

        public string LocalIdAddress { get; set; }
        public int? Progress { get; set; }
        public DateTime? LastContactUtc { get; set; }

        /// <summary>
        /// Created a new entity for an existing device.
        /// </summary>
        /// <param name="deviceId">The value identifying the device.</param>
        /// <param name="status">Delegate passed an instance of the created
        /// entity so additional behaviors can be applied.</param>
        /// <returns>Created entity with valid state.</returns>
        public static DeviceStatus ForExistingDevice(Guid deviceId, Action<DeviceStatus> status) 
        {
            if (deviceId == Guid.Empty)
                throw new ArgumentException("Device Id not specified.", nameof(deviceId));

            if (status == null)
                throw new ArgumentNullException(nameof(status));

            var entity = new DeviceStatus { DeviceId = deviceId };
            status(entity);            
            return entity;
        }

        /// <summary>
        /// Records the updated status of the device.
        /// </summary>
        public void RecordHeartbeat(DeviceState state, int uptimeSeconds, 
            string agentVersion, 
            string applicationVersion, 
            string rootFileSystemVersion)
        {
            // TODO:  Add validations...

            UptimeSeconds = uptimeSeconds;
            State = state;
            AgentVersion = agentVersion;
            ApplicationVersion = applicationVersion;
            RootFileSystemVersion = rootFileSystemVersion;
            LastContactUtc = DateTime.UtcNow;
        } 
    }
}




