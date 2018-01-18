using System;
using System.Linq;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using CommandLine;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Boondocks.Cli.Commands
{
    [Verb("app-deploy", HelpText = "Deploys an application to the server")]
    public class AppDeployCommand : CommandBase
    {
        [Option('a', "app-id", Required = true, HelpText = "The application uuid.")]
        public string ApplicationId { get; set; }

        [Option('i', "image", Required = true, HelpText = "The image id.")]
        public string Image { get; set; }

        [Option('d', "docker-uri", HelpText = "The docker server to pull from. Default is http://localhost:2375")]
        public string DockerUri { get; set; }

        [Option('n', "name", Required = true, HelpText = "The name of this version.")]
        public string Name { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            Guid? applicationId = ApplicationId.TryParseGuid();

            if (applicationId == null)
            {
                Console.WriteLine("Invalid app-id.");
                return 1;
            }

            string dockerUri = "http://localhost:2375";

            if (!string.IsNullOrWhiteSpace(DockerUri))
            {
                dockerUri = DockerUri;
            }

            //Create the docker client
            DockerClient dockerClient = new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();

            var listParameters = new ImagesListParameters()
            {
                All = true,
            };

            //Grab all of the images
            var images = await dockerClient.Images.ListImagesAsync(listParameters);

            //Try to find the image
            var image = images.FirstOrDefault(i => i.ID.Contains(Image));

            //Check to see if we found the image.
            if (image == null)
            {
                Console.WriteLine($"Unable to find image '{Image}'.");
                return 1;
            }

            //Get the download stream
            using (var sourceStream = await dockerClient.Images.SaveImageAsync(image.ID))
            {
                //Upload this joker
                var applicationVersion =  await context.Client.UploadApplicationVersionAsync(applicationId.Value, Name, image.ID, sourceStream);

                //Let the user know that it happened.
                Console.WriteLine($"ApplicationVersion {applicationVersion.Id} successfuly created.");
            }

            return 0;
        }
    }
}