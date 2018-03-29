using Boondocks.Device.Api.Models;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;
using System.Threading.Tasks;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("v1.0/device/healthchecks")]
    [AllowAnonymous]
    public class HealthCheckController : Controller
    {
        private readonly IMessagingService _messagingSrv;

        public HealthCheckController(IMessagingService messagingSrv)
        {
            _messagingSrv = messagingSrv;
        }

        [HttpGet]
        public async Task<MicroserviceHealthCheck> GetHealthCheck()
        {
            ServiceStatus status =  await _messagingSrv.DispatchAsync(GetHealthCheckStatus.Query);
            return new MicroserviceHealthCheck {
                DatabaseStatus = status.LastDataUpdates
            };
        }
    }
}