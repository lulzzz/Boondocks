using System;

namespace Boondocks.Device.Client.Models
{
    public class LogEventModel
    {
        public DateTime TimestampUtc { get; set; }
        public DateTime TimestampLocal { get; set; }
        public String Type { get; set; }
        public string Content { get; set; }
    }
}