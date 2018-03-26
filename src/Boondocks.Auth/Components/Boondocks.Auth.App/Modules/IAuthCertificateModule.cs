using Microsoft.IdentityModel.Tokens;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Auth.App.Modules
{
    /// <summary>
    /// Plugin module interface exposing the loaded private certificate key used
    /// to sign the authentication token. 
    /// </summary>
    public interface IAuthCertificateModule : IPluginModuleService
    {
        /// <summary>
        /// Returns the certification key.
        /// </summary>
        /// <returns>The x509 private key used for signing authorization tokens.</returns>
        X509SecurityKey GetPrivateKey();
    }
}