namespace Boondocks.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using ExtensionMethods;
    using Services.Management.Contracts;

    [Verb("build", HelpText = "Builds an application.")]
    public class BuildCommand : CommandBase
    {
        [Option('h', "dockerHost", Default= "http://localhost:2375", HelpText = "The docker endpoint to use for building.")]
        public string DockerEndpoint { get; set; }

        [Option('s', "source", Required = true, HelpText = "The source directory to build from.")]
        public string Source { get; set; }

        [Option('d', "deploy", Default = true, HelpText = "True to deploy, false to skip.")]
        public bool Deploy { get; set; }

        [Option('a', "application", HelpText = "The application to deploy this build to. Required if deploy=true.")]
        public string Application { get; set; }

        [Option('n', "name", HelpText = "The name to give this version.")]
        public string Name { get; set; }

        [Option('c', "make-current", HelpText = "Make this the current version of the application.", Default = true)]
        public bool MakeCurrent { get; set; }

        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Console.WriteLine("No name was specified.");
                return 1;
            }

            var tag = Name.Trim().ToLower();

            using (var temporaryFile = new TemporaryFile())
            {
                //Create the tar in a temporary place
                TarUtil.CreateTarGZ(temporaryFile.Path, Source);

                //Create the docker client
                var dockerClient = new DockerClientConfiguration(new Uri(DockerEndpoint)).CreateClient();

                //Open up the temp file
                using (var tarStream = File.OpenRead(temporaryFile.Path))
                {
                    //Set up the build
                    var imageBuildParmeters = new ImageBuildParameters
                    {
                        Tags = new List<string>
                        {
                            tag
                        }
                    };

                    BuildResult result;

                    //Build it!!!!!
                    using (var resultStream =
                        await dockerClient.Images.BuildImageFromDockerfileAsync(tarStream, imageBuildParmeters, cancellationToken))
                    {
                        //Deal with the result
                        result = await resultStream.ProcessBuildStreamAsync();
                    }

                    //Check to see if we have any errors
                    if (result.Errors.Any())
                    {
                        Console.WriteLine($"Build completed with {result.Errors.Count} errors.");

                        return 1;
                    }

                    //Let us deploy
                    if (Deploy)
                    {
                        return await DeployAsync(context, dockerClient, result, tag, cancellationToken);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

                    return 0;
                }
            }
        }

        private async Task<int> DeployAsync(CommandContext context, DockerClient dockerClient, BuildResult result,
            string tag, CancellationToken cancellationToken)
        {
            //Get the last id
            var imageId = result.Ids.LastOrDefault();

            if (string.IsNullOrWhiteSpace(imageId))
            {
                Console.Error.WriteLine("No image id was found. Unable to deploy.");

                return 1;
            }

            //Make sure we have an application name
            if (string.IsNullOrWhiteSpace(Application))
            {
                Console.Error.WriteLine("No application was specified.");
                return 1;
            }

            var application = await context.FindApplicationAsync(Application, cancellationToken);

            if (application == null)
            {
                return 1;
            }

            //Create the request
            var applicationUploadInfoRequest = new GetApplicationUploadInfoRequest()
            {
                ApplicationId = application.Id,
                ImageId = imageId,
                Name = tag
            };

            var applicationUploadInfo = await context.Client.ApplicationUpload.GetApplicationUploadInfo(applicationUploadInfoRequest, cancellationToken);

            if (applicationUploadInfo.CanUpload)
            {
                Console.WriteLine($"Deploying with imageid '{imageId}'...");

                var parameters = new ImagePushParameters
                {
                    ImageID = imageId,
                    Tag = tag
                };

                var target = $"{applicationUploadInfo.RegistryHost}/{applicationUploadInfo.Repository}";

                //Tag it!
                await dockerClient.Images.TagImageAsync(imageId, new ImageTagParameters
                {
                    RepositoryName = target,
                    Tag = tag
                }, cancellationToken);

                //Auth config (will have to include the token)
                var authConfig = new AuthConfig
                {
                    ServerAddress = applicationUploadInfo.RegistryHost
                };

                //Push it to the application registry!
                await dockerClient.Images.PushImageAsync(target, parameters, authConfig,
                    new Progress<JSONMessage>(p => { }), cancellationToken);

                //Let the service now about the new application version.
                var uploadRequest = new CreateApplicationVersionRequest
                {
                    ApplicationId = application.Id,
                    Name = tag,
                    ImageId = imageId,
                    Logs = result.ToString(),
                    MakeCurrent = MakeCurrent
                };

                //Upload the application version
                await context.Client.ApplicationVersions.UploadApplicationVersionAsync(uploadRequest, cancellationToken);               
            }
            else
            {
                Console.Error.WriteLine($"Warning: Unable to upload image - '{applicationUploadInfo.Reason}'.");
            }

            return 0;
        }

        

        

        
    }
}