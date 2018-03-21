using Boondocks.Auth.Api.Models;
using Boondocks.Auth.Api.Resources;
using NetFusion.Rest.Common;
using NetFusion.Rest.Server.Hal;

#pragma warning disable CS4014
namespace Boondocks.Auth.WebApi.Controllers
{
    public class AuthenticationRelations : HalResourceMap
    {
        // Adds HAL resource links to returned IdentityAuthResource responses.
        public override void OnBuildResourceMap()
        {
            Map<IdentityAuthResource>()
                .LinkMeta<AuthenticationController>(meta => {
                    meta.Url(RelationTypes.Self, (c, r) => c.Authenticate(default(AuthResourceModel), default(AuthCredentialModel)));
                });
        }
    }
}