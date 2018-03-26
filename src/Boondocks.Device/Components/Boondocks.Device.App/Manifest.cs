using NetFusion.Bootstrap.Manifests;

namespace Context.App
{
    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "2DACCE81-9DE2-490A-A06A-DF2CA70E0A14";
        public string Name => "Application Services Component";
        public string Description => "The plugin containing the Microservice's application services.";
    }
}
