using NetFusion.Bootstrap.Manifests;

namespace Boondocks.Base.Data
{

    public class Manifest : PluginManifestBase,
        ICorePluginManifest
    {
        public string PluginId => "8E0AA89E-E18F-4E23-BC5E-26ABED60C27F";
        public string Name => "Core Data Access";
        public string Description => "Components containing DB components and services.";
    }
}