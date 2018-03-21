using Boondocks.Auth.Api.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Auth.WebApi.Controllers
{
    /// <summary>
    /// Exposes a resource containing the entry-point API URIs for the Microservice.
    /// This allows clients requesting HAL to resource responses to fine the entry
    /// URIs they can use to start communication with service.
    /// </summary>
    [Route("api/boondocks/authentication/entry")]
    public class EntryController : Controller
    {
        [AllowAnonymous, HttpGet]
        public EntryPointResource GetEntryPoint() 
        {
            return new EntryPointResource {};
        }
    }
}