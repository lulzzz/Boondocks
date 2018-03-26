using NetFusion.Bootstrap.Manifests;

namespace Context.Infra
{
    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "BA26AB32-09DC-400D-8CEB-32542905E748";
        public string Name => "Infrastructure Contacts Application Component";
        public string Description => "The plugin containing the application Contacts infrastructure.";
    }
}
