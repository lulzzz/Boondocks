using Boondocks.Auth.Domain.Entities;
using NetFusion.Base.Plugins;
using System.Collections.Generic;

namespace Boondocks.Auth.Domain.Services
{
    /// <summary>
    /// Implement ion used to register one or more authentication providers responsible
    /// for handling authentication for on behalf of requesting services.
    /// </summary>
    public interface IAuthProviderRegistry : IKnownPluginType
    {
        /// <summary>
        /// Dictionary of provider registrations keyed by service name.
        /// </summary>
        IReadOnlyDictionary<string, ProviderRegistration> Registrations { get; }

        /// <summary>
        /// Instructs the registry to initialize it list of provider registrations.
        /// </summary>
        void Build();
    }
}