using NetFusion.Bootstrap.Manifests;

namespace Boondocks.Auth.WebApi.Bootstrap
{
    public class HostManifest : PluginManifestBase,
         IAppHostPluginManifest
    {
        public string PluginId => "2DEB2F24-5D28-418F-9C5E-B5A9AF42BC90";
        public string Name => "Contacts WebApi Host";
        public string Description => "WebApi host exposing Contacts REST/HAL based API.";
    }
}
