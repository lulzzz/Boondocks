using System;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using CommandLine;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace BoondocksCli.Commands
{
    [Verb("app-deploy", HelpText = "Deploys an application to the server")]
    public class AppDeployCommand : CommandBase
    {
        [Option('a', "app-id", Required = true, HelpText = "The application uuid.")]
        public string ApplicationId { get; set; }

        [Option('i', "image", Required = true, HelpText = "The image uuid.")]
        public string ImageUuid { get; set; }

        [Option('d', "docker-uri", HelpText = "The docker server to pull from. Default is http://localhost:2375")]
        public string DockerUri { get; set; }

        [Option('n', "name", HelpText = "The name of this version.")]
        public string Name { get; set; }

        public override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            Guid? applicationId = ApplicationId.ParseGuid();

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
                MatchName = ImageUuid
            };

            var images = await dockerClient.Images.ListImagesAsync(listParameters);

            if (images.Count == 0)
            {
                Console.WriteLine("No images found.");
                return 1;
            }

            if (images.Count > 1)
            {
                Console.WriteLine($"{images.Count} images found. Need just one!");
                return 1;
            }

            var image = images[0];

            //Get the download stream
            using (var sourceStream = await dockerClient.Images.SaveImageAsync(image.ID))
            {
                //Upload this joker
                var applicationVersion =  await context.Client.UploadApplicationVersionAsync(applicationId.Value, Name, sourceStream);

                Console.WriteLine($"ApplicationVersion {applicationVersion.Id} successfuly created.");
            }

            return 0;
        }
    }
}