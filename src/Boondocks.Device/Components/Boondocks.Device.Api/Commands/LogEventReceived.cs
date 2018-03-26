using Boondocks.Device.Api.Models;
using NetFusion.Messaging.Types;
using System;

namespace Boondocks.Device.Api.Commands
{
    /// <summary>
    /// Command sent when a request to write a log event is received.
    /// </summary>
    public class LogEventReceived : Command
    {
        private LogEventReceived() {}

        public Guid DeviceId { get; private set; }

        /// <summary>
        /// Indicates that the device events should be deleted. 
        /// </summary>
        public bool PurgeExisting { get; private set; }

        /// <summary>
        /// The log events to write. 
        /// </summary>
        public LogEventModel[] LogEvents { get; private set; } = Array.Empty<LogEventModel>();

        /// <summary>
        /// Creates a command containing a set of log events for a device.
        /// </summary>
        /// <param name="logEvents">Device log events.</param>
        /// <param name="purgeExisting">Indicates that any existing events associated with
        /// the device should be deleted before inserting the new events.</param>
        /// <returns>Created command.</returns>
        public static LogEventReceived HavingDetails(Guid deviceId, LogEventsModel logEvents,
            bool purgeExisting = false) =>
    
            new LogEventReceived { 
                DeviceId = deviceId,
                PurgeExisting = purgeExisting,
                LogEvents = logEvents.Events ?? throw new ArgumentNullException(nameof(logEvents))
            };

        /// <summary>
        /// Creates a command used to delete all log events associated with a device.
        /// </summary>
        /// <returns>Created command.</returns>
        public static LogEventReceived Purge(Guid deviceId) => 
            new LogEventReceived {
                DeviceId = deviceId, 
                PurgeExisting = true
            };
    }
}