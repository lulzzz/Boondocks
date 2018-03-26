using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Services;
using System;
using System.Collections.Generic;

namespace Boondocks.Auth.App.Providers
{
    /// <summary>
    /// Default provider registry base class used to add provider registrations to determine 
    /// the provider to be invoked when authenticating for a specific requesting service-API.
    /// </summary>
    public abstract class AuthProviderRegistry : IAuthProviderRegistry
    {
        // Lookup by service name to provider-registration.
        private Dictionary<string, ProviderRegistration> _registrations;

        public IReadOnlyDictionary<string, ProviderRegistration> Registrations { get; }

        public AuthProviderRegistry()
        {
            _registrations = new Dictionary<string, ProviderRegistration>();

            Registrations = _registrations;
        }

        /// <summary>
        /// Adds to the registry the provider used to preform the authentication for a
        /// service-API requesting authentication.
        /// </summary>
        /// <param name="serviceName">The name identifying the service-API.</param>
        /// <returns>Registration to set additional settings.</returns>
        protected ProviderRegistration Add<T>(string serviceApi)
            where T : IAuthProvider
        {
            var registration = new ProviderRegistration(serviceApi, typeof(T));
            if (_registrations.ContainsKey(serviceApi))
            {
                throw new InvalidOperationException(
                    $"The service-api named: {serviceApi} aready has a registered provider.");
            }
            _registrations[serviceApi] = registration;
            return registration;
        }

        /// <summary>
        /// Overridden by derived class to configure registry.
        /// </summary>
        public abstract void Build();
    }
}