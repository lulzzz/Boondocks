namespace Boondocks.Agent.Base.Tests
{
    using System.Collections.Generic;
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
        [MemberData(nameof(GetPositiveTestCases))]
        public void PositiveTest(string[] reserved, string[] current, EnvironmentVariable[] next)
        {
            var comparer = new EnvironmentVariableComparer(reserved, _logger);

            bool areSame = comparer.AreSame(current, next);

            Assert.True(areSame);
        }

        [Theory]
        [MemberData(nameof(GetNegativeTestCases))]
        public void NegativeTest(string[] reserved, string[] current, EnvironmentVariable[] next)
        {
            var comparer = new EnvironmentVariableComparer(reserved, _logger);

            bool areSame = comparer.AreSame(current, next);

            Assert.False(areSame);
        }

        public static IEnumerable<object[]> GetPositiveTestCases()
        {
            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=2" },
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "A", Value = "1" },
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                }
            };

            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=2", "RESERVED1=3" },
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "A", Value = "1" },
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                }
            };

            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=2" },
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                    new EnvironmentVariable() { Name = "A", Value = "1" },
                }
            };

            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=2", "RESERVED1=42" },
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "A", Value = "1" },
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                }
            };

            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=2"},
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "A", Value = "1" },
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                }
            };
        }

        public static IEnumerable<object[]> GetNegativeTestCases()
        {
            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=2" },
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "A", Value = "3" },
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                }
            };

            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=2", "RESERVED1=3" },
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "A", Value = "5" },
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                }
            };

            yield return new object[]
            {
                new string[] { "RESERVED1", "RESERVED2" },
                new string [] { "A=1", "B=7" },
                new EnvironmentVariable[]
                {
                    new EnvironmentVariable() { Name = "B", Value = "2" },
                    new EnvironmentVariable() { Name = "A", Value = "1" },
                }
            };
        }

       
    }
}
