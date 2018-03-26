using Boondocks.Auth.Domain.Entities;

namespace Boondocks.Auth.Domain.Services
{
    /// <summary>
    /// Service responsible for returning a x509 signed authentication 
    /// token for an authenticated caller.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Returns signed JWT token contained a set of claims for the
        /// authenticated caller.
        /// </summary>
        /// <param name="resourcePermissions">Information about the resources and the actions
        /// allows for the authenticated identity.</param>
        /// <returns>JWT signed token.</returns>
        string CreateClaimToken(ResourcePermission[] resourcePermissions);
    }
}
