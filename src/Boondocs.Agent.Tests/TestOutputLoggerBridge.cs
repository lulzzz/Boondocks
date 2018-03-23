namespace Boondocks.Agent.Base.Tests
{
    using System;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Xunit.Abstractions;

    public class TestOutputLoggerBridge :  ILogEventSink
    {
        private readonly ITestOutputHelper _output;

        public TestOutputLoggerBridge(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void Emit(LogEvent logEvent)
        {
            _output.WriteLine(logEvent.RenderMessage());
        }
    }
}