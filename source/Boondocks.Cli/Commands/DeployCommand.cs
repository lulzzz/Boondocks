using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Boondocks.Cli.Commands
{
    [Verb("deploy", HelpText = "Deploys an application to a registry.")]
    public class DeployCommand : CommandBase
    {
        [Option('d', "docker-endpoint", HelpText = "The docker endpoint to use for building.")]
        public string DockerEndpoint { get; set; }

        [Option('r', "registry-endpoint", HelpText = "The registry to publish this to")]
        public string RegistryEndpoint { get; set; }

        [Option('i', "image", HelpText = "The source directory to use when building the image.")]
        public string Image { get; set; }

        [Option('t', "tag", HelpText = "Tag.")]
        public string Tag { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            //Create the docker client
            DockerClient dockerClient = new DockerClientConfiguration(new Uri(DockerEndpoint)).CreateClient();

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

            var parameters = new ImagePushParameters()
            {
                ImageID = image.ID,
                Tag = Tag,
            };

            var authConfig = new AuthConfig()
            {
                ServerAddress = RegistryEndpoint,
            };

            //TODO: Get the repository name from the management service (there should be a repository per application).

            //Tag it!
            await dockerClient.Images.TagImageAsync(image.ID, new ImageTagParameters()
            {
                RepositoryName = "10.0.4.44:5000/my-repo",
                Tag = null
            });

            //Push it!
            await dockerClient.Images.PushImageAsync("10.0.4.44:5000/my-repo", parameters, authConfig, new Progress<JSONMessage>(p => Console.WriteLine(p.Status)));

            //var results = await dockerClient.Images.SearchImagesAsync(new ImagesSearchParameters()
            //{ 
            //    RegistryAuth = "http://10.0.4.44:5000",
            //    Term  = "microsoft"
            //});

            return 0;

        }
    }
}