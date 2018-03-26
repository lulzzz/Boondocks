namespace Boondocks.Agent.Base.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Docker.DotNet.Models;
    using Xunit;

    public class ContainerNameTests
    {
        [Theory]
        [InlineData("/name", "name")]
        [InlineData("/to", "to")]
        [InlineData("to", "to")]
        public void GetName(string rawName, string expected)
        {
            var result = DockerExtensions.GetContainerName(rawName);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("name", new [] { "/name", "/foo", "/bar" })]
        [InlineData("to", new [] { "/name", "/foo", "/bar", "/to" })]
        public void FindContainerByName(string name, string[] containerNames)
        {
            var containers = containerNames
                .Select(n => new ContainerListResponse()
                {
                    Names = new List<string>()
                    {
                        n
                    }
                });

            var container = containers.FindByName(name);

            Assert.NotNull(container);

            Assert.Contains(name, container.Names.Select(DockerExtensions.GetContainerName));
        }

        [Theory]
        [InlineData("name", new [] { "/asdfsdf", "/foo", "/bar" })]
        [InlineData("to", new [] { "/name", "/foo", "/bar", "/qwerwero" })]
        public void FindContainerByNameNegative(string name, string[] containerNames)
        {
            var containers = containerNames
                .Select(n => new ContainerListResponse()
                {
                    Names = new List<string>()
                    {
                        n
                    }
                });

            var container = containers.FindByName(name);

            Assert.Null(container);
        }

        [Theory]
        [InlineData(
            new [] { "a", "b"  },
            new [] { "/a", "/b", "/c"  },
            new [] { "/a", "/b"  })]
        public void FindContainersByNames(string[] toFind, string[] containerNames, string[] expectedNames)
        {
            var allContainers = containerNames
                .Select(n => new ContainerListResponse()
                {
                    Names = new List<string>() {n}
                });

            var foundContainers = allContainers.FindByNames(toFind);

            var foundNames = foundContainers
                .SelectMany(c => c.Names)
                .ToArray();

            foreach (var expectedName in expectedNames)
            {
                Assert.Contains(expectedName, foundNames);
            }
        }

        [Theory]
        [InlineData(
            new [] { "a", "b"  },
            new [] { "/foo", "/bar", "/c"  })]
        public void FindContainersByNamesNegative(string[] toFind, string[] containerNames)
        {
            var allContainers = containerNames
                .Select(n => new ContainerListResponse()
                {
                    Names = new List<string>() {n}
                });

            var foundContainers = allContainers.FindByNames(toFind);

            Assert.Empty(foundContainers);
        }
    }
}