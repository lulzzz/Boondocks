﻿namespace Boondocks.Cli.Commands
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
    using Services.DataAccess.Domain;
    using Services.Management.Contracts;

    [Verb("agent-build", HelpText = "Builds a agent and optionally uploads it.")]
    public class AgentBuildCommand : DockerCommandBase
    {
        [Value(0, Required = true, HelpText = "The source directory to build from.")]
        public string Source { get; set; }

        [Value(1, Required = true, HelpText = "The device type to use (e.g. 'RaspberryPi3')")]
        public string DeviceType { get; set; }

        [Value(2, HelpText = "The name to give this version.")]
        public string Name { get; set; }

        [Option('d', "deploy", Default = true, HelpText = "True to deploy, false to skip.")]
        public bool Deploy { get; set; }

        [Option('c', "make-current", HelpText = "Make this the current version of the application.", Default = true)]
        public bool MakeCurrent { get; set; }

        [Option('r', "registry-host", HelpText = "Allows the registry info from the server to be overridden.")]
        public string RegistryHost { get; set; }

        
        protected override async Task<int> ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Console.WriteLine("No name was specified.");
                return 1;
            }

            var tag =$"{DeviceType.ToLower()}-agent-{Name.Trim().ToLower()}";

            using (var temporaryFile = new TemporaryFile())
            {
                //Create the tar in a temporary place
                TarUtil.CreateTarGZ(temporaryFile.Path, Source);

                //Create the docker client
                using (var dockerClient = CreateDockerClient())
                {

                    //Open up the temp file
                    using (var tarStream = File.OpenRead(temporaryFile.Path))
                    {
                        //Set up the build
                        var imageBuildParmeters = new ImageBuildParameters
                        {
                            Tags = new List<string>
                            {
                                tag
                            },
                            Dockerfile = $"{DeviceType}.Agent.Dockerfile"
                        };

                        BuildResult result;

                        //Build it!!!!!
                        using (var resultStream =
                            await dockerClient.Images.BuildImageFromDockerfileAsync(tarStream, imageBuildParmeters,
                                cancellationToken))
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
        }

        private async Task<int> DeployAsync(CommandContext context, IDockerClient dockerClient, BuildResult result,
           string tag, CancellationToken cancellationToken)
        {
            //Get the last id
            var imageId = result.Ids.LastOrDefault();

            if (string.IsNullOrWhiteSpace(imageId))
            {
                Console.Error.WriteLine("No image id was found. Unable to deploy.");

                return 1;
            }

            if (string.IsNullOrWhiteSpace(DeviceType))
            {
                Console.Error.WriteLine("No device type was specified.");
                return 1;
            }

            DeviceType deviceType = await context.FindDeviceTypeAsync(DeviceType, cancellationToken);

            if (deviceType == null)
                return 1;

            //Create the request
            var uploadInforRequest = new GetAgentUploadInfoRequest()
            {
                DeviceTypeId = deviceType.Id,
                ImageId = imageId,
                Name = tag
            };

            var applicationUploadInfo = await context.Client.AgentUploadInfo.GetAgentUploadInfo(uploadInforRequest, cancellationToken);

            if (applicationUploadInfo.CanUpload)
            {
                Console.WriteLine($"Deploying '{tag}' with imageid '{imageId}'...");

                var parameters = new ImagePushParameters
                {
                    ImageID = imageId,
                    Tag = tag
                };

                string registryHost = RegistryHost ?? applicationUploadInfo.RegistryHost;

                var target = $"{registryHost}/{applicationUploadInfo.Repository}";

                //Tag it!
                await dockerClient.Images.TagImageAsync(imageId, new ImageTagParameters
                {
                    RepositoryName = target,
                    Tag = tag
                }, cancellationToken);

                //Auth config (will have to include the token)
                var authConfig = new AuthConfig
                {
                    ServerAddress = registryHost
                };

                Console.WriteLine($"Pushing to '{target}:{tag}'...");

                //Push it to the application registry!
                await dockerClient.Images.PushImageAsync(target, parameters, authConfig,
                    new Progress<JSONMessage>(p => { }), cancellationToken);

                //Let the service now about the new application version.
                var uploadRequest = new CreateAgentVersionRequest()
                {
                    DeviceTypeId = deviceType.Id,
                    Name = tag,
                    ImageId = imageId,
                    Logs = result.ToString(),
                    MakeCurrent = MakeCurrent
                };

                //Upload the application version
                await context.Client.AgentVersions.CreateAgentVersion(uploadRequest, cancellationToken);
            }
            else
            {
                Console.Error.WriteLine($"Warning: Unable to upload image - '{applicationUploadInfo.Reason}'.");
            }

            return 0;
        }
    }
}