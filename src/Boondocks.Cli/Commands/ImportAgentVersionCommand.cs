namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Docker.DotNet.Models;
    using ExtensionMethods;

    [Verb("import-agent-ver", HelpText = "Imports an agent version from hub.docker.com.")]
    public class ImportAgentVersionCommand : DockerCommandBase
    {
        [Option('d', "device-type", Required = true, HelpText = "The device type")]
        public string DeviceType { get; set; }

        [Option('t', "tag", Required = true, HelpText = "The tag of the agent version (e.g. boondocks-agent-raspberrypi31.3.2)")]
        public string Tag { get; set; }

        [Option("from-repo", Required = true, HelpText = "(e.g. boondocks/boondocks-agent-raspberrypi3")]
        public string FromRepository { get; set; }

        [Option("to-repo", Required = true, HelpText = "The repo to push to")]
        public string ToRepository { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            //Look up the device type
            var deviceType = await context.FindDeviceTypeAsync(DeviceType, cancellationToken);

            if (deviceType == null)
                return 1;

            //create the docker instance
            using (var dockerClient = CreateDockerClient())
            {
                string fromImage = $"{FromRepository}:{Tag}";

                Console.WriteLine($"Pulling from '{fromImage}'...");

                var imageCreateParameters = new ImagesCreateParameters
                { 
                    FromImage = fromImage,
                    //FromSrc = "https://registry-1.docker.io/",
                    //Repo = "boondocks/boondocks-agent-raspberrypi3",
                    //Tag = Tag
                };

                var localAuthConfig = new AuthConfig
                {
                    
                };
                
                //Pull the agent version 
                await dockerClient.Images.CreateImageAsync(
                    imageCreateParameters,
                    localAuthConfig,
                    new Progress<JSONMessage>(m => Console.WriteLine(m.ProgressMessage)), cancellationToken);

                //Create the image tag paramters
                var imageTagParameters = new ImageTagParameters
                {
                    RepositoryName = ToRepository,
                    Tag = Tag,
                };

                Console.WriteLine($"Tagging the image with '{imageTagParameters}'...");

                //Tag the image
                await dockerClient.Images.TagImageAsync(fromImage, imageTagParameters, cancellationToken);

                string toImage = $"{ToRepository}:{Tag}";

                //Get docker information from the management service
                var imagePushParameters = new ImagePushParameters
                { 
                   // Tag = toImage
                };

                Console.WriteLine($"Pushing '{toImage}'...");

                //Push to our registry
                await dockerClient.Images.PushImageAsync(
                    toImage,
                    imagePushParameters,
                    localAuthConfig,
                    new Progress<JSONMessage>(m => Console.WriteLine(m.ProgressMessage)),
                    cancellationToken);

                //TODO: Let the management service know that we uploaded it

                Console.WriteLine("Done.");

            }


            return 0;
        }
    }
}