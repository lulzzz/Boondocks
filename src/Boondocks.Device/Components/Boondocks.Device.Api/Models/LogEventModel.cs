using Boondocks.Device.Domain.Entities;
using System;

namespace Boondocks.Device.Api.Models
{
    /// <summary>
    /// Model populated when a request is made to record a
    /// log event.
    /// </summary>
    public class LogEventModel
    {
        public DateTime TimestampUtc { get; set; }
        public DateTime TimestampLocal { get; set; }
        public LogEventType Type { get; set; }
        public string Content { get; set; }
    }
}