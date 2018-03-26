using System;

namespace Boondocks.Device.Domain.Entities
{
    /// <summary>
    /// Domain entity
    /// </summary>        
    public class ApplicationLog
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The id of the device in question.
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// The type of the log event.
        /// </summary>
        public LogEventType Type { get; set; }

        /// <summary>
        /// The console content.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The local time of the log event.
        /// </summary>
        public DateTime CreatedLocal { get; set; }

        public DateTime CreatedUtc { get; set;}

        public void Validate()
        {

        }
    }
}