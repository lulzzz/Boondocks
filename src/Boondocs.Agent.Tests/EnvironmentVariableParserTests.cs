namespace Boondocks.Agent.Base.Tests
{
    using Model;
    using Xunit;

    public class EnvironmentVariableParserTests
    {
        [Theory]
        [InlineData("foo=bar", "foo", "bar")]
        [InlineData("ace=1", "ace", "1")]
        [InlineData("ace", "ace", "")]
        [InlineData("ace=", "ace", "")]
        [InlineData("", "", "")]
        [InlineData(null, "", "")]
        public void ParseTest(string source, string expexectedName, string expectedValue)
        {
            var result = EnvironnmentVariableParser.Parse(source);

            Assert.NotNull(result);

            Assert.Equal(expexectedName, result.Name);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}