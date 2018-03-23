using System;
using System.Threading.Tasks;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("api/v1/boondocks/device")]
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

        [HttpGet("configuration")]
        public Task<DeviceConfiguration> GetCurrentConfiguration()
        {
            var query = new GetDeviceConfiguration(_context.DeviceId);
            return _messagingSrv.DispatchAsync(query);
        }

        [HttpGet("images/application/{id}/download")]
        [Produces(typeof(ImageDownloadModel))]
        public async Task<IActionResult> GetApplicationDownloadInfo(Guid id)
        {
            var imageInfo = await _messagingSrv.DispatchAsync(new GetApplicationImageInfo(id));

            // if (imageInfo.NoResult)
            // {
            //     return NotFound();
            // }

            return Ok(imageInfo);
        }

        [HttpGet(template: "images/agent/{id}/download")]
        public async Task<IActionResult> GetAgentDownloadInfo(Guid id)
        {
            var imageInfo = await _messagingSrv.DispatchAsync(new GetAgentImageInfo(id));

            // if (imageInfo.NoResult)
            // {
            //     return NotFound();
            // }

            return Ok(imageInfo);
        }

    }
}