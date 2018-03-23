using System.Threading.Tasks;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("api/v1/boondocks/device/configuration")]
    public class DeviceConfigurationController : Controller
    {
        private IDeviceContext _deviceContext;
        private IMessagingService _messagingSrv;

        public DeviceConfigurationController(
            IDeviceContext deviceContext,
            IMessagingService messagingSrv)
        {
            _deviceContext = deviceContext;
            _messagingSrv = messagingSrv;
        }

        [HttpGet()]
        public Task GetCurrentConfiguration()
        {
            var query = new CurrentDeviceConfiguration(_deviceContext.DeviceId);
            return _messagingSrv.DispatchAsync(query);
        }
    }
}