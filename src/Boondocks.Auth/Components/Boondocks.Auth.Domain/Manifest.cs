using NetFusion.Bootstrap.Manifests;

namespace Context.Domain
{

    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "DE6C5158-8054-4B47-BE4D-4C636EE69DB6";
        public string Name => "Boondocks Domain Model application-component.";
        public string Description => 
            "Component containing the domain model representing the authentication process.";
    }

}
