namespace Boondocks.Cli.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Agent.Shared;
    using CommandLine;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    [Verb("deploy-agent", HelpText = "Deploys an agent directly to a device.")]
    public class DeployAgentCommand : CommandBase
    {
        [Option('i', "image", Required = true, HelpText = "The id of the image to deploy.")]
        public string Image { get; set; }

        [Option('s', "source", Default = "http://localhost:2375/", HelpText = "The source docker instance where the agent image resides.")]
        public string Source { get; set; }

        [Option('t', "target", Required = true, HelpText = "The target device docker endpoint.")]
        public string Target { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            using (IDockerClient sourceDockerClient = new DockerClientConfiguration(new Uri(Source)).CreateClient())
            using (IDockerClient targetDockerClient = new DockerClientConfiguration(new Uri(Target)).CreateClient())
            {
                //Find the image.
                var sourceImages = await sourceDockerClient.Images.ListImagesAsync(new ImagesListParameters() {All = true}, cancellationToken);

                var sourceImage = sourceImages
                    .FirstOrDefault(i => i.ID.Contains(Image));

                if (sourceImage == null)
                {
                    Console.Error.WriteLine($"Unable to find source image '{Image}' at '{Source}'.");
                    return 1;
                }

                Console.WriteLine($"Found image '{sourceImage.ID}'. Copying to target...");

                //Copy the image.
                using (var sourceImageStream = await sourceDockerClient.Images.SaveImageAsync(sourceImage.ID, cancellationToken))
                {
                    await targetDockerClient.Images.LoadImageAsync(new ImageLoadParameters(), sourceImageStream,
                        new Progress<JSONMessage>(p => Console.WriteLine(p.Status)), cancellationToken);
                }

                //Ditch the containers that might cause a problem.
                await targetDockerClient.ObliterateContainerAsync(DockerConstants.AgentContainerName, cancellationToken: cancellationToken);
                await targetDockerClient.ObliterateContainerAsync(DockerConstants.AgentContainerOutgoingName, cancellationToken: cancellationToken);

                //Create the container factory
                var containerFactory = new AgentDockerContainerFactory();

                //Create the container itself
                var createContainerResponse = await containerFactory.CreateContainerAsync(targetDockerClient, sourceImage.ID, cancellationToken);

                Console.WriteLine($"Container '{createContainerResponse.ID}' created. Starting container...");

                //Start the container.
                bool started = await targetDockerClient.Containers.StartContainerAsync(createContainerResponse.ID,
                    new ContainerStartParameters(), cancellationToken);

                if (started)
                {
                    Console.WriteLine("Agent container started.");
                    return 0;
                }
                
                Console.Error.WriteLine("Agent container failed to start.");
                return 1;
            }
        }
    }
}