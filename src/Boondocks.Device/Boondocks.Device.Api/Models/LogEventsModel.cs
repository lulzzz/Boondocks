namespace Boondocks.Device.Api.Models
{
    /// <summary>
    /// A set of log events populated when a request is received
    /// to write logs for the calling device.
    /// </summary>
    public class LogEventsModel
    {
        public LogEventModel[] Events { get; set;  }
    }
}