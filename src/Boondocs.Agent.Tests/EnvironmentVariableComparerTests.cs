using System;

namespace Boondocs.Agent.Tests
{
    using System.Collections.Generic;
    using Boondocks.Agent.Model;
    using Boondocks.Services.Device.Contracts;
    using Xunit;

    public class EnvironmentVariableComparerTests
    {
        [Theory]
        [MemberData(nameof(GetPositiveTestCases))]
        public void PositiveTest(string[] reserved, string[] current, EnvironmentVariable[] next)
        {
            var comparer = new EnvironmentVariableComparer(reserved);

            bool areSame = comparer.AreSame(current, next);

            Assert.True(areSame);
        }

        [Theory]
        [MemberData(nameof(GetNegativeTestCases))]
        public void NegativeTest(string[] reserved, string[] current, EnvironmentVariable[] next)
        {
            var comparer = new EnvironmentVariableComparer(reserved);

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
