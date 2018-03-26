using NetFusion.Bootstrap.Manifests;

namespace Context.Api
{
    // This class identifies the assembly as a plug-in that will be discovered
    // by the bootstrap process.
    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "0902BBD4-0F0F-4E92-B183-744568BE3375";
        public string Name => "Boondocks Authentication API";
        public string Description => "Contains the API for issuing authentication requests.";
    }
}
