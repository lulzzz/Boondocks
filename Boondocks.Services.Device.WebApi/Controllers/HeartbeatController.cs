using System;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApi.Common;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/Heartbeat")]
    //[Authorize]
    public class HeartbeatController : DeviceControllerBase
    {
        /// <summary>
        /// A heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HeartbeatResponse Post(HeartbeatRequest request)
        {
            return new HeartbeatResponse()
            {
                ConfigurationVersion = Guid.NewGuid()
            };
        }
    }
}