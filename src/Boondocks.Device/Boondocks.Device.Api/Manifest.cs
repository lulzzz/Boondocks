using NetFusion.Bootstrap.Manifests;

namespace Context.Api
{
    // This class identifies the assembly as a plug-in that will be discovered
    // by the bootstrap process.
    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "D92EBECF-4E99-4FDB-A124-922D38D8DC98";
        public string Name => "Microservice API Component";
        public string Description => "The plugin containing the Microservice's public API.";
    }
}
