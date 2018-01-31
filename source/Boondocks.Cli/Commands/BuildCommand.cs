﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using CommandLine;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json.Linq;

namespace Boondocks.Cli.Commands
{
    [Verb("build", HelpText = "Builds an application.")]
    public class BuildCommand : CommandBase
    {
        [Option('h', "dockeHost", HelpText = "The docker endpoint to use for building.")]
        public string DockerEndpoint { get; set; }

        [Option('s', "source", Required = true, HelpText = "The source directory to build from.")]
        public string Source { get; set; }

        [Option('d', "deploy", Default = true, HelpText = "True to deploy, false to skip.")]
        public bool Deploy { get; set; }

        [Option('a', "Application", HelpText = "The application to deploy this build to. Required if deploy=true.")]
        public string Application { get; set; }

        [Option('n', "name", HelpText = "The name to give this version.")]
        public string Name { get; set; }

        protected override async Task<int> ExecuteAsync(ExecutionContext context)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Console.WriteLine("No name was specified.");
                return 1;
            }

            string tag = Name.Trim().ToLower();

            using (var temporaryFile = new TemporaryFile())
            {
                //Create the tar in a temporary place
                TarUtil.CreateTarGZ(temporaryFile.Path, Source);

                //Create the docker client
                DockerClient dockerClient = new DockerClientConfiguration(new Uri(DockerEndpoint)).CreateClient();

                //Open up the temp file
                using (var tarStream = File.OpenRead(temporaryFile.Path))
                {
                    //Set up the build
                    var imageBuildParmeters = new ImageBuildParameters()
                    {
                        Tags = new List<string>()
                        {
                            tag
                        }
                    };

                    //Build it!!!!!
                    using (var resultStream = await dockerClient.Images.BuildImageFromDockerfileAsync(tarStream, imageBuildParmeters))
                    {
                        //Deal with the result
                        var result = await ProcessResult(resultStream);

                        //Check to see if we have any errors
                        if (result.Errors.Any())
                        {
                            Console.WriteLine($"Build completed with {result.Errors.Count} errors.");

                            return 1;
                        }
                      
                        //Let us deploy
                        if (Deploy)
                        {
                            return await DeployAsync(context, dockerClient, result, tag);
                        }

                        return 0;
                    }
                }
            }
        }

        private async Task<int> DeployAsync(ExecutionContext context, DockerClient dockerClient, BuildResult result, string tag)
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

            var application =  await context.Client.GetApplicationAsync(Application);

            if (application == null)
            {
                Console.WriteLine($"Unable to find application '{Application}'.");
                return 1;
            }

            var applicationUploadInfo = await context.Client.GetApplicationUploadInfo(application.Id);

            Console.WriteLine($"Deploying with imageid '{imageId}'...");

            var parameters = new ImagePushParameters()
            {
                ImageID = imageId,
                Tag = tag,
            };

            string target = $"{applicationUploadInfo.RegistryHost}/{applicationUploadInfo.Repository}";

            //Tag it!
            await dockerClient.Images.TagImageAsync(imageId, new ImageTagParameters()
            {
                RepositoryName = target,
                Tag = tag
            });

            //Auth config (will have to include the token)
            var authConfig = new AuthConfig()
            {
                ServerAddress = applicationUploadInfo.RegistryHost,
            };

            //Push it!
            await dockerClient.Images.PushImageAsync(target, parameters, authConfig, new Progress<JSONMessage>(p => { }));

            //TODO: Upload the version information
            

            return 0;
        }

        private async Task<BuildResult> ProcessResult(Stream responseStream)
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

            try
            {
                using (var streamReader = new StreamReader(responseStream))
                {
                    string line = await streamReader.ReadLineAsync();

                    while (!string.IsNullOrEmpty(line))
                    {
                        //Parse the line
                        JObject parsedLine = JObject.Parse(line);

                        //Attempt to handle this line
                        var handler = handlers.FirstOrDefault(h => h(parsedLine, result));

                        //Check to see if it was handled.
                        if (handler == null)
                        {
                            //This line wasn't handled.
                            Console.WriteLine(line);
                        }

                        //Read the next line
                        line = await streamReader.ReadLineAsync();
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (SocketException)
            {
            }

            return result;
        }

        private static bool HandleStream(JObject parsedLine, BuildResult result)
        {
            var streamProperty = parsedLine.Property("stream");

            if (streamProperty != null)
            {
                string formatted = $"{streamProperty.Value}";

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
                    string formatted = $"{idProperty.Value}";

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
                string formatted = $"{property}";

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
        }
    }
}