namespace Boondocks.Agent.Base.Model
{
    using System;
    using System.IO;

    /// <summary>
    /// Responsible for generating the paths we use throughout the agent.
    /// </summary>
    public class LinuxPathFactory : PathFactoryBase
    {
        public override string DockerEndpoint 
        {
            get
            {
                var raw = Environment.GetEnvironmentVariable("DOCKER_SOCKET") ?? "/var/run/balena.sock";

                return "unix:/" + Path.Combine(HostRoot, raw);
            }
        }

        private string HostRoot => "/mnt/root";

        private string BootMountpoint => Environment.GetEnvironmentVariable("BOOT_MOUNTPOINT") ?? "/mnt/boot";

        /// <summary>
        /// The path of the device configuration file.
        /// </summary>
        public override string DeviceConfigFile => $"{HostRoot}{BootMountpoint}/{DeviceConfigFilename}";       
    }
}