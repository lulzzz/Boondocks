using NetFusion.Bootstrap.Manifests;

namespace Boondocks.Base.Data
{

    public class Manifest : PluginManifestBase,
        ICorePluginManifest
    {
        public string PluginId => "2DACCE81-4444-490A-A06A-DF2CA70E0A14";
        public string Name => "Core component containing DB components and services.";
        public string Description => "";
    }
}