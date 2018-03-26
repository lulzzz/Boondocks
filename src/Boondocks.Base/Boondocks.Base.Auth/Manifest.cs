using NetFusion.Bootstrap.Manifests;

namespace Boondocks.Base.Auth
{

    public class Manifest : PluginManifestBase,
        ICorePluginManifest
    {
        public string PluginId => "2DACCE81-5555-490A-A06A-DF2CA70E0A14";
        public string Name => "Core component containing Authentication components and services.";
        public string Description => "";
    }
}