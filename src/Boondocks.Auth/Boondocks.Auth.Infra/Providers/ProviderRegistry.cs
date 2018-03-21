using Boondocks.Auth.App.Providers;

namespace Boondocks.Auth.Infra
{
    /// <summary>
    /// Discovered by the AuthProviderModule when the application is bootstrapped.
    /// Specifies when an authentication request is requested from a given boondocks
    /// service, the provider that should preform the authentication.
    /// </summary>
    public class ProviderRegistry : AuthProviderRegistry
    {
        public override void Build()
        {
            Add<DeviceAuthProvider>("device-api")
                .ForValidIssuers("boondocks-issuer")
                .ForValidAudiences("boondocks-api");

             Add<ActiveDirectoryAuthProvider>("management-api");
        }
    }
}