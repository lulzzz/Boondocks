using System.Threading.Tasks;
using Boondocks.Auth.Api.Models;
using Boondocks.Auth.Api.Resources;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Rest.Server.Hal;

#pragma warning disable CS4014
namespace Boondocks.Auth.WebApi.Controllers
{
    public class EntryRelations : HalResourceMap
    {
        // Adds HAL links to the entry service API methods from which communication
        // can start with the service.
        public override void OnBuildResourceMap()
        {
            Map<EntryPointResource>()
                .LinkMeta<AuthenticationController>(meta => {
                    meta.UrlTemplate<AuthResourceModel, AuthCredentialModel, 
                        Task<IActionResult>>("authenticate", c => c.Authenticate);
                });
        }
    }
}