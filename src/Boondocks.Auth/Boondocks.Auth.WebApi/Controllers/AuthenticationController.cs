using Boondocks.Auth.WebApi.ActionResults;
using Boondocks.Auth.Api.Commands;
using Boondocks.Auth.Api.Models;
using Boondocks.Auth.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NetFusion.Web.Mvc.Metadata;
using Boondocks.Auth.Api.Resources;

namespace Boondocks.Auth.WebApi.Controllers
{
    /// <summary>
    /// Web API controller user to authenticate an identity for another
    /// requesting API service.
    /// </summary>
    [Route("api/v1/boondocks/authentication"), GroupMeta(nameof(AuthenticationController))]
    public class AuthenticationController : Controller
    {
        private readonly IMessagingService _messagingSrv;

        public AuthenticationController(
            IMessagingService messagingSrv)
        {
            _messagingSrv = messagingSrv;
        }

        /// <summary>
        /// Called by a service-API to authenticate an identity.
        /// </summary>
        /// <param name="resourceModel">Optional.  Contains information about a set of resources
        /// and the actions that are to be allowed on the resource to be validated.</param>
        /// <param name="credentialModel">The credentials used to validate an identity.</param>
        /// <returns>Result containing outcome of the authentication.</returns>
        [AllowAnonymous, HttpPost, ActionMeta(nameof(Authenticate))]
        public async Task<IActionResult> Authenticate(
            [FromQuery()]AuthResourceModel resourceModel, 
            [FromBody]AuthCredentialModel credentialModel)
        {
            resourceModel = resourceModel ?? new AuthResourceModel();
            credentialModel = credentialModel ?? new AuthCredentialModel();

            // Create command from the submitted models and send to application layer:
            var command = new AuthenticateCaller(credentialModel, resourceModel);
            AuthResult authResult = await _messagingSrv.SendAsync(command);

            // Based on the result, set the HTTP response code, response header and body:
            if (authResult.IsInvalidCredentialContext)
            {
                return BadRequest(authResult.Reason);
            }

            if (! authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            var resource = IdentityAuthResource.FromResult(command, authResult);
            return new OkJwtAuthResult(authResult.JwtSignedToken, resource);
        } 
    }
}
