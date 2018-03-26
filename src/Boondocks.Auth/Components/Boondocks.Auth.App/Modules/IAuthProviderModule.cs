using Autofac;
using Boondocks.Auth.Domain.Services;
using NetFusion.Bootstrap.Plugins;

namespace Boondocks.Auth.App.Modules
{
    /// <summary>
    /// Plugin module responsible for resolving the authentication provider used to authenticate
    /// and identity for a requesting service-API.
    /// </summary>
    public interface IAuthProviderModule : IPluginModuleService
    {
        /// <summary>
        /// For the service-API name specified within the context, returns the authentication provider
        /// responsible for preforming the authentication.
        /// </summary>
        /// <param name="currentScope">Reference to the current lifetime scope used to dependency-inject the provider.</param>
        /// <param name="serviceName">The name of the service requesting authentication.</param>
        /// <returns>Instance of the provider.</returns>
        IAuthProvider GetServiceAuthProvider(ILifetimeScope currentScope, string serviceName);
    }
}