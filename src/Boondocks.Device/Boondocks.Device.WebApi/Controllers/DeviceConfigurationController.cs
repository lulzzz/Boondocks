using System.Threading.Tasks;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App;
using Boondocks.Device.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("api/v1/boondocks/device/configuration")]
    public class DeviceConfigurationController : Controller
    {
        private IDeviceContext _context;
        private IMessagingService _messagingSrv;

        public DeviceConfigurationController(
            IDeviceContext context,
            IMessagingService messagingSrv)
        {
            _context = context;
            _messagingSrv = messagingSrv;
        }

        [HttpGet()]
        public Task GetCurrentConfiguration()
        {
            var query = new CurrentDeviceConfiguration(_context.DeviceId);
            return _messagingSrv.DispatchAsync(query);
        }
    }
}