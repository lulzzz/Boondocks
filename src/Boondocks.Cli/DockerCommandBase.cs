namespace Boondocks.Cli
{
    using System;
    using CommandLine;
    using Docker.DotNet;

    public abstract class DockerCommandBase : CommandBase
    {
        [Option('h', "dockerHost", Default = "http://localhost:2375", HelpText = "The docker endpoint to use for this operation.")]
        public string DockerEndpoint { get; set; }

        protected IDockerClient CreateDockerClient()
        {
            //create the configuration
            var configuration = new DockerClientConfiguration(new Uri(DockerEndpoint));

            //Create the docker client
            return configuration.CreateClient();
        }
    }
}