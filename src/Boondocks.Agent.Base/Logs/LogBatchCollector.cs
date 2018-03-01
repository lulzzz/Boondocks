namespace Boondocks.Agent.Base.Logs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Timers;
    using Boondocks.Base;
    using Serilog;
    using Services.Contracts;
    using Services.Device.Contracts;
    using Services.Device.WebApiClient;

    public class LogBatchCollector
    {
        private readonly DeviceApiClient _deviceApiClient;
        private readonly ILogger _logger;

        //The maximum amount of time to go before sending all held messages.
        private const int TimerInterval = 10 * 1000;

        //The maximum number of items to allow in a batch.
        private const int EmitBatchMaximumSize = 100;

        private readonly List<DockerLogEvent> _events = new List<DockerLogEvent>();
        private readonly Timer _timer;
        private readonly AsyncLock _lock = new AsyncLock();
        private bool _isFirstBatch = true;

        public LogBatchCollector(DeviceApiClient deviceApiClient, ILogger logger)
        {
            _deviceApiClient = deviceApiClient ?? throw new ArgumentNullException(nameof(deviceApiClient));
            _logger = logger.ForContext(GetType());
            _timer = new Timer(TimerInterval)
            {
                Enabled = false,
                AutoReset = false
            };

            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer elapsed");

            EmitAsync();

            _timer.Start();
        }

        public async Task AddAsync(DockerLogEvent logEvent)
        {
            bool emit = false;

            using (await _lock.LockAsync())
            {
                _events.Add(logEvent);

                if (_events.Count >= EmitBatchMaximumSize)
                {
                    emit = true;
                }
            }

            _timer.Start();

            if (emit)
            {
                EmitAsync();
            }
        }

        public async void EmitAsync()
        {
            try
            {
                using (await _lock.LockAsync())
                {
                    //Check to see if we have any held events
                    if (_events.Count > 0)
                    {
                        //Create a request
                        var request = new SubmitApplicationLogsRequest
                        {
                            Events = _events.ToArray(),
                            IsFirst = _isFirstBatch
                        };

                        _logger.Verbose($"Emitting application logs with {_events.Count} events.");

                        //Upload the logs!!!!!!
                        await _deviceApiClient.ApplicationLogs.SubmitLogsAsync(request);

                        _isFirstBatch = false;

                        //Clear out the events that we just sent. Keep the same list instance so that we
                        // don't keep allocating memory.
                        _events.Clear();
                    }
                    else
                    {
                        _logger.Verbose("No events to emit.");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Warning(e, "Emit Error: " + e);
            }
        }
    }
}