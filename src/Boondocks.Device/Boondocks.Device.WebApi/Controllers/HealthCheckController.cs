using System.Threading.Tasks;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.Domain;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("api/v1/boondocks/device/healthcheck")]
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