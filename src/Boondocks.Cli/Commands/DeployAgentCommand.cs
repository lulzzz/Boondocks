namespace Boondocks.Cli.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using ExtensionMethods;
    using Boondocks.Agent.Base;
    using Services.Management.WebApiClient;

    [Verb("deploy-agent", HelpText = "Deploys an agent directly to a device.")]
    public class DeployAgentCommand : CommandBase
    {
        [Option('d', "device-type", Required = true, HelpText = "The device type to use (e.g. 'RaspberryPi3')")]
        public string DeviceType { get; set; }

        [Option('v', "version", Required = true, HelpText = "The version of the agent to deploy.")]
        public string Version { get; set; }

        [Option('s', "source", Default = "http://localhost:2375/", HelpText = "The source docker instance where the agent image resides.")]
        public string Source { get; set; }

        [Option('t', "target", Required = true, HelpText = "The target device docker endpoint.")]
        public string Target { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            var deviceType = await context.FindDeviceTypeAsync(DeviceType, cancellationToken);

            if (deviceType == null)
                return 1;

            var agentVersion = await context.FindAgentVersion(deviceType.Id, Version, cancellationToken);

            if (agentVersion == null)
                return 1;

            using (IDockerClient sourceDockerClient = new DockerClientConfiguration(new Uri(Source)).CreateClient())
            using (IDockerClient targetDockerClient = new DockerClientConfiguration(new Uri(Target)).CreateClient())
            {
                Console.WriteLine("Saving image to target...");

                //Copy the image.
                using (var sourceImageStream = await sourceDockerClient.Images.SaveImageAsync(agentVersion.ImageId, cancellationToken))
                {
                    await targetDockerClient.Images.LoadImageAsync(new ImageLoadParameters(), sourceImageStream,
                        new Progress<JSONMessage>(p => Console.WriteLine(p.Status)), cancellationToken);
                }

                Console.WriteLine("Removing target agent container(s)...");

                //Ditch the containers that might cause a problem.
                await targetDockerClient.ObliterateContainerAsync(DockerConstants.AgentContainerName, cancellationToken: cancellationToken);
                await targetDockerClient.ObliterateContainerAsync(DockerConstants.AgentContainerOutgoingName, cancellationToken: cancellationToken);

                Console.WriteLine("Creating agent container...");

                //Create the container factory
                var containerFactory = new AgentDockerContainerFactory();

                //Create the container itself
                var createContainerResponse = await containerFactory.CreateContainerForDirectAsync(targetDockerClient, agentVersion.ImageId, cancellationToken);

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