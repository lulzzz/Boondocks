using System.Threading.Tasks;
using Boondocks.Auth.Api.Models;
using Boondocks.Auth.Api.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Rest.Resources.Hal;
using NetFusion.Rest.Server.Hal;

namespace Boondocks.Auth.WebApi.Controllers
{
    /// <summary>
    /// Exposes a resource containing the entry-point API URIs for the Microservice.
    /// This allows clients requesting HAL to resource responses to fine the entry
    /// URIs they can use to start communication with service.
    /// </summary>
    [Route("v1.0/auth/entries")]
    [AllowAnonymous]
    public class ApiEntryControllerV1_0 : Controller
    {
        [AllowAnonymous, HttpGet]
        public ApiEntryResourceV1_0 GetEntryPoint() 
        {
            return new ApiEntryResourceV1_0 {};
        }
    }

    /// <summary>
    /// Resource containing the entry-point URIs for the microservice.
    /// </summary>
    public class ApiEntryResourceV1_0 : HalEntryPointResource
    {
        public ApiEntryResourceV1_0()
        {
            Version = typeof(ApiEntryResourceV1_0).Assembly
                .GetName().Version.ToString();   
        }
    }

    public class ApiEntryPointRelations : HalResourceMap
    {
        // Adds HAL links to the entry service API methods from which communication
        // can start with the service.
        public override void OnBuildResourceMap()
        {
            Map<ApiEntryResourceV1_0>()
                .LinkMeta<AuthenticationController>(meta => {
                    meta.UrlTemplate<AuthResourceModel, AuthCredentialModel, 
                        Task<IActionResult>>("authenticate", c => c.Authenticate);
                });
        }
    }
}