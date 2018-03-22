using System.Threading.Tasks;
using Boondocks.Device.Api.Commands;
using Boondocks.Device.Api.Models;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("api/v1/boondocks/device/heartbeat")]
    public class HeartbeatController : Controller
    {
        private readonly IMessagingService _messagingSrv;

        public HeartbeatController(IMessagingService messagingSrv)
        {
            _messagingSrv = messagingSrv;
        }

        /// <summary>
        /// Proceses the device heartbeat so we know that the device is still alive. We also send back the 
        /// configuration version of the device so that the device knows if it needs to update itself.
        /// </summary>
        /// <param name="model">The posted model containing the updated heeartbeat information.</param>
        /// <returns>Model</returns>
        [HttpPost]
        public async Task<HeartbeatResponseModel> UpdateDeviceHeartbeat([FromBody]DeviceHeartbeatModel model)
        {
            var command = HeartbeatReceived.FromDevice(model);
            var deviceVersion = await _messagingSrv.SendAsync(command);

            return new HeartbeatResponseModel {
                ConfigurationVersion = deviceVersion.ConfigurationVersion
            };
        }
    }
}