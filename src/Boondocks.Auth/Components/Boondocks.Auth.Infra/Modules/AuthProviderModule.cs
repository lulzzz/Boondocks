using Autofac;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetFusion.Bootstrap.Plugins;
using System.Collections.Generic;
using System.Linq;

namespace Boondocks.Auth.App.Modules
{
    /// <summary>
    /// Plug-in module that scans for a single IAuthProviderRegister instance
    /// used to determine the IAuthProvider to call when authenticating for a
    /// specific named service. 
    /// </summary>
    public class AuthProviderModule : PluginModule,
        IAuthProviderModule
    {
        // Set by NetFusion to all concrete classes:
        public IEnumerable<IAuthProviderRegistry> Registries { get;  private set; }

        // Service Name Requesting Authentication --> Registration Lookup:
        private IReadOnlyDictionary<string, ProviderRegistration> _serviceAuthProviders;

        public override void Initialize()
        {
            if (Registries.Count() > 1)
            {
                Context.Logger.LogError(
                    LogEvents.ProviderRegistrationError,
                    "More than one provider registry was found within loaded plugin assemblies.");
                return;
            }

            var registry = Registries.FirstOrDefault();
            if (registry == null)
            {
                Context.Logger.LogError(
                    LogEvents.ProviderRegistrationError, 
                    "No provider registries were found within loaded plugin assemblies.");

                _serviceAuthProviders = new Dictionary<string, ProviderRegistration>();
                return;
            }

            registry.Build();
            _serviceAuthProviders = registry.Registrations;
        }

        // Add each authentication provider to the dependency-injection container.
        public override void RegisterComponents(ContainerBuilder builder)
        {
            foreach(ProviderRegistration registration in _serviceAuthProviders.Values)
            {
                // Get optional token validation parameters for registered provider.  This will
                // be the case if the providers received a signed token identifying the caller.
                TokenValidationParameters validationParams = GetValidationParams(registration);

                // When the provider is resolved, set the token validation parameters.
                builder.RegisterType(registration.ProviderType)
                    .InstancePerLifetimeScope()
                    .OnActivated(e => {
                        IAuthProvider provider = (IAuthProvider)e.Instance;
                        provider.SetTokenValidationParameters(validationParams);
                    });
            }
        }

        private TokenValidationParameters GetValidationParams(ProviderRegistration registration)
        {
            return new TokenValidationParameters {
                ValidIssuers = registration.ValidIssuers,
                ValidAudiences = registration.ValidAudiences
            };
        }

        public IAuthProvider GetServiceAuthProvider(ILifetimeScope currentScope, string serviceName)
        {
            ProviderRegistration registration = GetServiceRegistration(serviceName);

            // No registration found for named service.
            if (registration == null)
            {
                return null;
            }

            // Get the provider type associated with the service requesting authentication.
            return (IAuthProvider)currentScope.Resolve(registration.ProviderType);
        }

        private ProviderRegistration GetServiceRegistration(string serviceName)
        {
            _serviceAuthProviders.TryGetValue(serviceName, out ProviderRegistration registration);
            return registration;
        }

        public override void Log(IDictionary<string, object> moduleLog)
        {
            foreach (var authProvider in _serviceAuthProviders)
            {
                var serviceName = authProvider.Key;
                var registration = authProvider.Value;

                moduleLog[serviceName] = new {
                    ServiceRequestingAuthToken = serviceName,
                    AuthProvider = registration.ProviderType.FullName,
                    OptionalValidIssuers = registration.ValidIssuers,
                    OptionalValidAudiences = registration.ValidAudiences
                };
            }
        }
    }
}