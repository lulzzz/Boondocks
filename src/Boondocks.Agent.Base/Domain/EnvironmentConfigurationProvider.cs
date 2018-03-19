namespace Boondocks.Agent.Base.Domain
{
    using System;
    using Interfaces;

    /// <summary>
    /// Pulls configuration values directly from the environment
    /// </summary>
    public class EnvironmentConfigurationProvider : IEnvironmentConfigurationProvider
    {
        /// <summary>
        /// How to connect to the docker (balena) endpoint
        /// </summary>
        public string DockerSocket => Environment.GetEnvironmentVariable("DOCKER_SOCKET") ?? "/var/run/balena.sock";

        public string BootMountpoint => Environment.GetEnvironmentVariable("BOOT_MOUNTPOINT") ?? "/mnt/boot" ;
    }
}