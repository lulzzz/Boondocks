using NetFusion.Bootstrap.Manifests;

namespace Boondocks.Device.WebApi.Bootstrap
{
    public class HostManifest : PluginManifestBase,
         IAppHostPluginManifest
    {
        public string PluginId => "FFA982FE-C93B-42C3-BF91-E0FA1F7FB626";
        public string Name => "Contacts WebApi Host";
        public string Description => "WebApi host exposing Contacts REST/HAL based API.";
    }
}
