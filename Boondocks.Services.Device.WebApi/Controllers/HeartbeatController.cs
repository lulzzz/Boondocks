using System;
using System.Text;
using System.Threading.Tasks;
using Boondocks.Services.Device.Contracts;
using Boondocks.Services.Device.WebApi.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("v1/Heartbeat")]
    public class HeartbeatController : DeviceControllerBase
    {
        /// <summary>
        /// A heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(HeartbeatRequest request)
        {
            if (Authorization == null)
            {
                return Unauthorized();
            }

            return Ok(new HeartbeatResponse()
            {
                EnvironmentVariables = new EnvironmentVariable[]
                {
                    new EnvironmentVariable()
                    {
                        Name = "device-key",
                        Value = Authorization.DeviceKey
                    },
                }
            });
        }
    }
}