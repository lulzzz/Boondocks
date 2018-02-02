namespace Boondocks.Cli.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Docker.DotNet;
    using Docker.DotNet.Models;
    using ExtensionMethods;
    using Newtonsoft.Json.Linq;
    using Services.Management.Contracts;
    using ExecutionContext = Cli.ExecutionContext;

    [Verb("build", HelpText = "Builds an application.")]
    public class BuildCommand : CommandBase
    {
        [Option('h', "dockerHost", HelpText = "The docker endpoint to use for building.")]
        public string DockerEndpoint { get; set; }

        [Option('s', "source", Required = true, HelpText = "The source directory to build from.")]
        public string Source { get; set; }

        [Option('d', "deploy", Default = true, HelpText = "True to deploy, false to skip.")]
        public bool Deploy { get; set; }

        [Option('a', "Application", HelpText = "The application to deploy this build to. Required if deploy=true.")]
        public string Application { get; set; }

        [Option('n', "name", HelpText = "The name to give this version.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context, CancellationToken cancellationToken)
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
                        result = await ProcessResultAsync(resultStream);
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

                    return 0;
                }
            }
        }

        private async Task<int> DeployAsync(ExecutionContext context, DockerClient dockerClient, BuildResult result,
            string tag, CancellationToken cancellationToken)
        {
            //Get the last id
            var imageId = result.Ids.LastOrDefault();

            if (string.IsNullOrWhiteSpace(imageId))
            {
                Console.WriteLine("No image id was found. Unable to deploy.");

                return 1;
            }

            //Make sure we have an application name
            if (string.IsNullOrWhiteSpace(Application))
            {
                Console.WriteLine("No application was specified.");
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

                try
                {
                    //Push it!
                    await dockerClient.Images.PushImageAsync(target, parameters, authConfig,
                        new Progress<JSONMessage>(p => { }));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Temporarily ignoring error from push: {e.Message}");
                }

                var uploadRequest = new CreateApplicationVersionRequest
                {
                    ApplicationId = application.Id,
                    Name = tag,
                    ImageId = imageId,
                    Logs = result.ToString()
                };

                //Upload the application version
                await context.Client.ApplicationVersions.UploadApplicationVersionAsync(uploadRequest, cancellationToken);               
            }
            else
            {
                Console.WriteLine($"Warning: Unable to upload image - '{applicationUploadInfo.Reason}'.");
            }

            return 0;
        }

        private async Task<BuildResult> ProcessResultAsync(Stream responseStream)
        {
            //This response will get built up.
            var result = new BuildResult();

            //Create a list of line handlers
            var handlers = new Func<JObject, BuildResult, bool>[]
            {
                HandleAux,
                HandleStream,
                HandleError
            };
           
            using (var streamReader = new StreamReader(responseStream))
            {
                var line = await streamReader.ReadLineAsync();

                while (!string.IsNullOrEmpty(line))
                {
                    try
                    {
                        //Parse the line
                        var parsedLine = JObject.Parse(line);

                        //Attempt to handle this line
                        var handler = handlers.FirstOrDefault(h => h(parsedLine, result));

                        //Check to see if it was handled.
                        if (handler == null)
                        {
                            Console.WriteLine(line);

                            //Add this as a raw message
                            result.Messages.Add(line);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Unable to parse {line}: {ex.Message}");
                    }

                    try
                    {
                        //Read the next line
                        line = await streamReader.ReadLineAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                        
                }
            }

            return result;
        }

        private static bool HandleStream(JObject parsedLine, BuildResult result)
        {
            var streamProperty = parsedLine.Property("stream");

            if (streamProperty != null)
            {
                var formatted = $"{streamProperty.Value}";

                Console.Write(formatted);
                result.Messages.Add(formatted);

                return true;
            }

            return false;
        }

        private static bool HandleAux(JObject parsedLine, BuildResult result)
        {
            var auxProperty = parsedLine.Property("aux");

            if (auxProperty != null)
            {
                var idProperty = (auxProperty.Value as JObject)?.Property("ID");

                if (idProperty != null)
                {
                    var formatted = $"{idProperty.Value}";

                    result.Ids.Add(formatted);

                    Console.WriteLine("==============================================================");
                    Console.WriteLine(formatted);
                    Console.WriteLine("==============================================================");

                    result.Messages.Add(formatted);
                }

                return true;
            }

            return false;
        }

        private static bool HandleError(JObject parsedLine, BuildResult result)
        {
            var property = parsedLine.Property("error");

            if (property != null)
            {
                var formatted = $"{property}";

                Console.Write(formatted);
                result.Errors.Add(formatted);

                return true;
            }

            return false;
        }

        private class BuildResult
        {
            public IList<string> Ids { get; } = new List<string>();

            public IList<string> Messages { get; } = new List<string>();

            public IList<string> Errors { get; } = new List<string>();

            public override string ToString()
            {
                var output = new StringBuilder();

                foreach (var message in Messages) output.Append(message);

                return output.ToString();
            }
        }
    }
}