using System.Threading.Tasks;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Repositories;

namespace Boondocks.Auth.Infra.Repositories 
{
    public class LdapAuthRepository : ILdapAuthRepository
    {
        public Task<bool> UserAllowedAccess(string userName)
        {
            return Task.FromResult(true);
        }

        public Task<ResourcePermission[]> GetUserAccessAsync(string username, ResourcePermission[] resourceAccess)
        {
            // The current requirement is that if the user is found within a list of granted users
            // and they have been authenticated, they have full access to registry resources.  This
            // can be changed if needed so currently returned the requested access items.
            return Task.FromResult(resourceAccess);
        }       
    }
}