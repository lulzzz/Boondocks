using Boondocks.Base.Auth;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;
using NetFusion.Web.Mvc.Metadata;
using System;
using System.Threading.Tasks;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("v1.0/device"), 
        GroupMeta(nameof(DeviceConfigurationController))]
    public class DeviceConfigurationController : Controller
    {
        private IDeviceContext _context;
        private IMessagingService _messagingSrv;

        public DeviceConfigurationController(
            IDeviceContext context2,
            IDeviceContext context,
            IMessagingService messagingSrv)
        {
            _context = context;
            _messagingSrv = messagingSrv;
        }

        [HttpGet("configurations"), 
            ActionMeta(nameof(DeviceConfiguration))]
        public Task<DeviceConfiguration> GetConfiguration()
        {
            var query = new GetDeviceConfiguration(_context.DeviceId);
            return _messagingSrv.DispatchAsync(query);
        }

        [HttpGet("images/applications/{id}/download"), 
            ActionMeta(nameof(GetAppDownloadInfo)),
            Produces(typeof(ImageDownloadModel))]
        public async Task<IActionResult> GetAppDownloadInfo(Guid id)
        {
            var imageInfo = await _messagingSrv.DispatchAsync(new GetApplicationImageInfo(id));

            // if (imageInfo.NoResult)
            // {
            //     return NotFound();
            // }

            return Ok(imageInfo);
        }

        [HttpGet(template: "images/agents/{id}/download"), 
            ActionMeta(nameof(GetAgentDownloadInfo))]
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