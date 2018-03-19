namespace Boondocks.Agent.Base.Model
{
    using System.IO;

    public class WindowsPathFactory : PathFactoryBase
    {
        private string Root => @"c:\Boondocks";

        public override string DeviceConfigFile => Path.Combine(Root, DeviceConfigFilename);

        public override string DockerEndpoint => "http://localhost:2375";
    }
}