namespace Boondocks.Agent.Logs
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using Serilog;
    using Shared;


    public class ApplicationLogSucker
    {
        private readonly IDockerClient _dockerClient;
        private readonly ILogger _logger;

        private const int RetrySeconds = 5;
        private const double LogSlewSeconds = 1.0;
        private readonly LogBatchCollector _batchCollector = new LogBatchCollector();

        public ApplicationLogSucker(IDockerClient dockerClient, ILogger logger)
        {
            _dockerClient = dockerClient ?? throw new ArgumentNullException(nameof(dockerClient));
            _logger = logger.ForContext(GetType());
        }

        public async Task SuckAsync(CancellationToken cancellationToken)
        {
            DateTime? lastTimestamp = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                var parameters = new ContainerLogsParameters()
                {
                    ShowStderr = true,
                    ShowStdout = true,
                    Follow = true,
                    Timestamps = true,
                };

                //We don't want to keep getting the same old log entries, so we grab this.
                if (lastTimestamp != null)
                {
                    parameters.Since = (lastTimestamp.Value.Add(TimeSpan.FromSeconds(LogSlewSeconds)).ToUnixEpochTime()).ToString();
                }

                try
                {
                    using (var logStream = await _dockerClient.Containers.GetContainerLogsAsync(DockerConstants.ApplicationContainerName, parameters, cancellationToken))
                    using (var streamReader = new LogStreamReader(logStream))
                    {
                        //Read the log event
                        DockerLogEvent logEvent = await streamReader.ReadAsync(cancellationToken);

                        //When this is null, the stream is to be closed.
                        while (logEvent != null)
                        {
                            //Keep track of this so we don't keep sucking down the same log entries.
                            lastTimestamp = logEvent.TimestampUtc;

                            //Console.WriteLine($"  [{logEvent.TimestampUtc}] {logEvent.Type} - {logEvent.Content}");

                            //Add this to the collector.
                            await _batchCollector.AddAsync(logEvent);

                            logEvent = await streamReader.ReadAsync(cancellationToken);
                        }
                    }

                    _logger.Verbose("Application log stream closed.");
                    
                }
                catch (TaskCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                }

                Console.WriteLine("Waiting before trying to contact the server again...");
                await Task.Delay(TimeSpan.FromSeconds(RetrySeconds), cancellationToken);
            }
            
        }
    }
}