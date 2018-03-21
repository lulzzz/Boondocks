using System;

namespace Boondocks.Auth.Domain.Entities
{
    /// <summary>
    /// Represents a registration specifying the provider type to call 
    /// for authenticating specific service-API.
    /// </summary>
    public class ProviderRegistration
    {
        // The name of a requesting service allowed authentication requests.
        public string ServiceApi { get; }

        // The provider implementing the authentication logic.
        public Type ProviderType { get; }

        public ProviderRegistration(
            string serviceName,
            Type providerType)
        {
            if (string.IsNullOrWhiteSpace(serviceName)) 
                throw new ArgumentException("Service name not specified", nameof(serviceName));

            ServiceApi = serviceName;
            ProviderType = providerType ?? throw new ArgumentNullException(nameof(providerType));
        }

        // The token issuer name must match one of these values to considered valid if a signed
        // token is being used to identify the requesting client. 
        public string[] ValidIssuers { get; private set; }

        // The token audience name must match one of these values to be considered valid if a signed
        // token is being used to identify the requesting client.
        public string[] ValidAudiences { get; private set; }
        
        /// <summary>
        /// Used to specify one or more issuer names considered to be valid if present in token
        /// submitted to identify the caller.
        /// </summary>
        /// <param name="values">Issuer names.</param>
        /// <returns>Registration for method chaining.</returns>
        public ProviderRegistration ForValidIssuers(params string[] values)
        {
            ValidIssuers = values ?? throw new ArgumentNullException(nameof(values));
            return this;
        }

        /// <summary>
        /// Used to specify one or more audience names considered to be valid if present in token
        /// submitted to identify the caller.
        /// </summary>
        /// <param name="values">Audience names.</param>
        /// <returns>Registration for method chaining.</returns>
        public ProviderRegistration ForValidAudiences(params string[] values)
        {
            ValidAudiences = values ?? throw new ArgumentNullException(nameof(values));
            return this;
        }
    }
}