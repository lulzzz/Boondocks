using NetFusion.Bootstrap.Manifests;

namespace Boondocks.Base.Auth
{
    public class Manifest : PluginManifestBase,
        ICorePluginManifest
    {
        public string PluginId => "AA5C7B1D-FA55-4B37-96DA-9D7A71C74890";
        public string Name => "Core Authentication.";
        public string Description => "Contains common classes and service for device authentication.";
    }
}