using System;
using System.Collections.Generic;
using System.Linq;

namespace Boondocks.Auth.Domain.Entities
{
    /// <summary>
    /// Contains information provided by the service-API requesting the authentication token.
    /// </summary>
    public class AuthContext
    {
        private AuthContext() {}

        // The name identifying the service-API requesting authentication token.
        public string RequestingApi { get; private set; }
        public IDictionary<string, string> Credentials { get; private set; }

        // Optional information about the resource to be granted access.
        public string ResourceOwner { get; private set; }
        public ResourcePermission[] Resources { get; set; } = Array.Empty<ResourcePermission>();
        public bool IsValidResourceScope { get; set; } = true;

        // Returns new context instance.
        public static AuthContext FromService(string requestingApi, IDictionary<string, string> credentials)
            => new AuthContext {
                RequestingApi = requestingApi,
                Credentials = credentials
            };

        // Returns new context with information about the resource to which
        // access should be granted.
        public AuthContext ForResource(string owner, string[] scope)
        {
            var context = FromService(RequestingApi, Credentials);
            context.ResourceOwner = owner;
            context.Resources = BuildAccessFromScope(scope).ToArray();

            return context;
        }

        // When Docker return returns a HTTP 401 status code, it specifies the following header:
        // Www-Authenticate: Bearer realm="https://auth.docker.io/token",service="registry.docker.io",scope="repository:samalba/my-app:pull,push"
        // When a client calls this authentication service, it should relay the following as a query string parameter:
        // scope="repository:samalba/my-app:pull,push" - Note:  there can be multiple scopes specified.
        private IEnumerable<ResourcePermission> BuildAccessFromScope(string[] scope)
        {
            IEnumerable<string[]> scopes = scope.Select(s => s.Split(':'));
            IsValidResourceScope = scopes.All(p => p.Length == 3);

            foreach(string[] resourceScope in scopes)
            {
                yield return new ResourcePermission(
                    type: resourceScope[0],
                    name: resourceScope[1],
                    actions: resourceScope[2].Split(','));
            }
        }
    }
}