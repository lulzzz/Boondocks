namespace Boondocks.Cli.Commands
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CommandLine;
    using Docker.DotNet.Models;
    using ExtensionMethods;
    using Services.DataAccess.Domain;
    using Services.Management.Contracts;

    [Verb("agent-ver-import", HelpText = "Imports an agent version from hub.docker.com.")]
    public class AgentVersionImportCommand : DockerCommandBase
    {
        [Option('d', "device-type", Required = true, HelpText = "The device type")]
        public string DeviceType { get; set; }

        [Value(0, Required = true, HelpText = "(e.g. boondocks/boondocks-agent-raspberrypi3")]
        public string FromRepository { get; set; }

        [Value(1, Required = true, HelpText = "The tag of the agent version (e.g. boondocks-agent-raspberrypi3v1.0.0)")]
        public string Tag { get; set; }

        [Option('r', "registry-host", HelpText = "Allows the registry info from the server to be overridden.")]
        public string RegistryHost { get; set; }
        
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
                };

                var localAuthConfig = new AuthConfig
                {
                    
                };
                
                //Pull the agent version 
                await dockerClient.Images.CreateImageAsync(
                    imageCreateParameters,
                    localAuthConfig,
                    new Progress<JSONMessage>(m => Console.WriteLine(m.ProgressMessage)), cancellationToken);

                var imageInspection = await dockerClient.Images.InspectImageAsync(fromImage, cancellationToken);

                if (imageInspection == null)
                {
                    Console.WriteLine($"Unable to find image '{fromImage}' for inspection.");
                    return 1;
                }

                var getAgentUploadInfoRequest = new GetAgentUploadInfoRequest
                {
                    ImageId = imageInspection.ID,
                    DeviceTypeId = deviceType.Id,
                    Name = Tag
                };

                var agentUploadInfo =
                    await context.Client.AgentUploadInfo.GetAgentUploadInfo(getAgentUploadInfoRequest,
                        cancellationToken);

                if (!agentUploadInfo.CanUpload)
                {
                    Console.WriteLine($"Unable to upload: {agentUploadInfo.Reason}");
                    return 1;
                }

                string registryHost = RegistryHost ?? agentUploadInfo.RegistryHost;

                //Create the image tag paramters
                var imageTagParameters = new ImageTagParameters
                {
                    RepositoryName = $"{registryHost}/{agentUploadInfo.Repository}",
                    Tag = Tag,
                };

                Console.WriteLine($"Tagging the image with '{imageTagParameters}'...");

                //Tag the image
                await dockerClient.Images.TagImageAsync(fromImage, imageTagParameters, cancellationToken);

                string toImage = $"{registryHost}/{agentUploadInfo.Repository}:{Tag}";

                Console.WriteLine($"Pushing '{toImage}'...");

                //Push to our registry
                await dockerClient.Images.PushImageAsync(
                    toImage,
                    new ImagePushParameters(),
                    localAuthConfig,
                    new Progress<JSONMessage>(m => Console.WriteLine(m.ProgressMessage)),
                    cancellationToken);

                //TODO: Let the management service know that we uploaded it
                var createAgentVersionRequest = new CreateAgentVersionRequest
                {
                    DeviceTypeId = deviceType.Id,
                    ImageId = imageInspection.ID,
                    MakeCurrent = false,
                    Name = Tag,
                    Logs = "Imported"
                };

                //Create the version
                AgentVersion agentVersion = await context.Client.AgentVersions.CreateAgentVersion(createAgentVersionRequest, cancellationToken);

                //And we're done
                Console.WriteLine($"Version {agentVersion.Id} created.");
            }
    
            return 0;
        }
    }
}