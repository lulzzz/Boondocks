using Boondocks.Auth.Api.Commands;
using Boondocks.Auth.Domain.Entities;
using NetFusion.Messaging.Types;
using NetFusion.Rest.Resources.Hal;
using System.Linq;

namespace Boondocks.Auth.Api.Resources
{
    /// <summary>
    /// Resource defining the public API for an authentication response.
    /// Contains an embedded resource collection containing the allowed
    /// actions for the submitted resources.
    /// </summary>
    public class IdentityAuthResource : HalResource 
    {
        public string CorrelationId { get; set; }
        public bool IsAuthenticated { get; set; }

        public IdentityAuthResource()
        {

        }

        public static IdentityAuthResource FromResult(AuthenticateCaller command, AuthResult authResult)
        {
            var resource = new IdentityAuthResource {
                CorrelationId = command.GetCorrelationId(),
                IsAuthenticated = authResult.IsAuthenticated
            };

            var accessResources = authResult.ResourcePermissions
                .Select(e => new AccessResource {
                    Type = e.Type,
                    Name = e.Name,
                    Actions = e.Actions
                }).ToArray();

            resource.Embed(accessResources, "resource-access")
;            return resource;
        }
    }
}