using System;
using System.Threading.Tasks;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Api.Queries;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;

namespace Boondocks.Device.WebApi.Controllers
{
    [Route("api/v1/boondocks/device/images")]
    public class ImageController : Controller
    {
        private IMessagingService _messagingSrv;

        public ImageController(IMessagingService messagingSrv)
        {
            _messagingSrv = messagingSrv;
        }

        [HttpGet(template: "agent/{id}/download")]
        [Produces(typeof(ImageDownloadModel))]
        public async Task<IActionResult> GetAgentDownloadInfo(Guid id)
        {
            var imageInfo = await _messagingSrv.DispatchAsync(new AgentImageInfo(id));

            if (imageInfo.NoResult)
            {
                return NotFound();
            }

            return Ok(imageInfo);
        }

        [HttpGet("application/{id}/download")]
        [Produces(typeof(ImageDownloadModel))]
        public async Task<IActionResult> GetApplicationDownloadInfo(Guid id)
        {
            var imageInfo = await _messagingSrv.DispatchAsync(new ApplicationImageInfo(id));

            if (imageInfo.NoResult)
            {
                return NotFound();
            }

            return Ok(imageInfo);
        }
    }
}