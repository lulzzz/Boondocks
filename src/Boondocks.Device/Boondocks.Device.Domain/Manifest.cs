using NetFusion.Bootstrap.Manifests;

namespace Context.Domain
{

    public class Manifest : PluginManifestBase,
        IAppComponentPluginManifest
    {
        public string PluginId => "B9AA7429-AC79-4420-8A73-324D1366DD3B";
        public string Name => "Domain Model Component";
        public string Description => "The component containing the Microservice's domain model.";
    }

}
