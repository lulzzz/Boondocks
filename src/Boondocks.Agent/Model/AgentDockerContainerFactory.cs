namespace Boondocks.Agent.Model
{
    using System.Collections.Generic;
    using Docker.DotNet.Models;

    internal class AgentDockerContainerFactory : DockerContainerFactory
    {
        protected override string CreateName()
        {
            return DockerConstants.AgentContainerName;
        }

        protected override CreateContainerParameters GetCreationParameters(string imageId, Config config)
        {
            var parameters = base.GetCreationParameters(imageId, config);

            parameters.HostConfig.Mounts = new List<Mount>()
            {
                new Mount()
                {
                    Type = "bind",
                    Source = "/mnt/boot/device.config",
                    Target = "/mnt/boot/device.config",
                }
            };

            parameters.HostConfig.Binds = new List<string>()
            {
                "/var/run/balena.sock:/var/run/balena.sock",
            };

            parameters.Env = new List<string>
            {
                "DOCKER_ENDPOINT=unix://var/run/balena.sock"
            };

            parameters.Volumes = new Dictionary<string, EmptyStruct>()
            {
                {"/data", new EmptyStruct()},
            };

            return parameters;
        }
    }
}