namespace Boondocks.Agent.Base.AgentLogging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Serilog.Events;
    using Serilog.Sinks.PeriodicBatching;
    using Services.Device.WebApiClient;

    internal class AgentLogSink : PeriodicBatchingSink
    {
        public AgentLogSink(int batchSizeLimit, TimeSpan period) 
            : base(batchSizeLimit, period)
        {
        }

        public AgentLogSink(int batchSizeLimit, TimeSpan period, int queueLimit) 
            : base(batchSizeLimit, period, queueLimit)
        {
        }

        protected override Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            if (DeviceApiClient == null)
            {
                Console.Error.WriteLine("Unable to emit batch of log entries. No DeviceApiClient has been specified yet.");
            }
            else if (events == null || !events.Any())
            {
                Console.WriteLine("No events to emit.");
            }
            else
            {
                //TODO: Emit the log entries
                var transformed = events.Select(e => new 
                {
                    CreatedUtc = e.Timestamp.ToUniversalTime(),
                    CreatedLocal = e.Timestamp.ToLocalTime(),
                    Message = e.RenderMessage(),
                    Level = e.Level
                }).ToArray();

                //TODO: Get this to the server!
                Console.WriteLine($"Quasi emitting {transformed.Length} events to the server.");
            }

            return Task.CompletedTask;
        }

        public DeviceApiClient DeviceApiClient { get; set; }
    }
}