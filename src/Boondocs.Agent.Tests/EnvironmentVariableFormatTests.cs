namespace Boondocks.Agent.Base.Tests
{
    using Services.Device.Contracts;
    using Xunit;

    public class EnvironmentVariableFormatTests
    {
        [Theory]
        [InlineData("foo", "1", "foo=1")]
        [InlineData("bar", "2", "bar=2")]
        public void FormatTest(string name, string value, string expected)
        {
            var environentVariable = new EnvironmentVariable
            {
                Name = name,
                Value = value
            };

            var variables = new[]
            {
                environentVariable
            };

            var formatted = variables.FormatForDevice();

            Assert.Single(formatted);

            Assert.Equal(expected, formatted[0]);
        }
    }
}