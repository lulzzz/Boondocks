using NetFusion.Bootstrap.Manifests;

namespace Context.Infra
{
    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "B89A2796-C734-4586-9167-DFC0458B8172";
        public string Name => "Infrastructure Boondocks Authorization Component";
        public string Description => 
            "Implements authenticating a caller for a given service and the " +
            "generation of a signed JWT token.";
    }
}
