using NetFusion.Bootstrap.Manifests;

namespace Boondocks.Auth.App
{
    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "671674D6-7D14-4DC0-94A0-B1085B878C23";
        public string Name => "Boondocks Authentication Application Services Component";
        public string Description => "The plugin containing the Microservice's application implementation.";
    }
}
