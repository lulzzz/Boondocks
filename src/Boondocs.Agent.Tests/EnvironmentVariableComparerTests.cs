namespace Boondocks.Agent.Base.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Model;
    using Serilog;
    using Services.Device.Contracts;
    using Xunit;
    using Xunit.Abstractions;

    public class EnvironmentVariableComparerTests
    {
        private readonly ILogger _logger;
        private readonly TestOutputLoggerBridge _outputSink;

        public EnvironmentVariableComparerTests(ITestOutputHelper output)
        {
            _outputSink = new TestOutputLoggerBridge(output);

            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Sink(_outputSink)
                .CreateLogger();
        }

        [Theory]
        [InlineData(new [] { "A=1" }, new [] { "A=1"})]
        [InlineData(new [] { "A=1", "B=2" }, new [] { "A=1", "B=2" })]
        [InlineData(new [] { "B=2", "A=1" }, new [] { "A=1", "B=2" })]
        [InlineData(new [] { "A=1", "B=2", "C=3" }, new [] { "A=1", "B=2","C=3" })]
        public void PositiveTest(string[] current, string[] next)
        {
            var comparer = new EnvironmentVariableComparer(_logger);

            EnvironmentVariable[] parsedCurrent = current
                .Select(EnvironnmentVariableParser.Parse)
                .ToArray();

            EnvironmentVariable[] parsedNext = next
                .Select(EnvironnmentVariableParser.Parse)
                .ToArray();

            bool areSame = comparer.AreSame(parsedCurrent, parsedNext);

            Assert.True(areSame);
        }

        [Theory]
        [InlineData(new string[] { }, new [] { "A=2"})]
        [InlineData(new string[] { "A=1" }, new string[] {})]
        [InlineData(new string[] { "A=1" }, new [] { "A=2"})]
        [InlineData(new string[] { "A=1", "B=2" }, new [] { "A=1", "B=3" })]
        [InlineData(new string[] { "B=2", "A=1" }, new [] { "A=0", "B=2" })]
        [InlineData(new string[] { "A=1", "B=2", "C=3" }, new [] { "A=1", "B=42","C=3" })]
        public void NegativeTest(string[] current, string[] next)
        {
            var comparer = new EnvironmentVariableComparer(_logger);

            EnvironmentVariable[] parsedCurrent = current
                .Select(EnvironnmentVariableParser.Parse)
                .ToArray();

            EnvironmentVariable[] parsedNext = next
                .Select(EnvironnmentVariableParser.Parse)
                .ToArray();

            bool areSame = comparer.AreSame(parsedCurrent, parsedNext);

            Assert.False(areSame);
        }
    }
}
