using System.Threading.Tasks;
using Boondocks.Auth.Domain.Entities;

namespace Boondocks.Auth.Domain.Repositories
{
    /// <summary>
    /// For a given identified user, returns their allows registry actions.
    /// </summary>
    public interface ILdapAuthRepository
    {
        /// <summary>
        /// Determines if a user is permitted access.  
        /// </summary>
        /// <param name="userName">The user name to check.</param>
        /// <returns>True if access allowed.  Otherwise, False.</returns>
        Task<bool> UserAllowedAccess(string userName);

        /// <summary>
        /// For an authenticated user, determines the registry access allowed.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="requestingAccess">An array of registry resources for which access is being requested.</param>
        /// <returns>A subset of the requested access registry items to which the device has access.</returns>
        Task<ResourcePermission[]> GetUserAccessAsync(string username, ResourcePermission[] requestingAccess);
    }
}