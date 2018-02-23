namespace Boondocks.Agent.Logs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Timers;
    using Base;

    public class LogBatchCollector
    {
        private const int TimerInterval = 10 * 1000;

        private const int EmitBatchMaximumSize = 5;

        private readonly List<DockerLogEvent> _events = new List<DockerLogEvent>();

        private readonly Timer _timer;

        private readonly AsyncLock _lock = new AsyncLock();

        private bool _isFirstBatch = true;

        public LogBatchCollector()
        {
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
                    if (_events.Count > 0)
                    {
                        Console.WriteLine($"Emitting {_events.Count} ");

                        _isFirstBatch = false;

                        _events.Clear();
                    }
                    else
                    {
                        Console.WriteLine("No events to emit.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Emit Error: " + e);
            }
        }
    }
}