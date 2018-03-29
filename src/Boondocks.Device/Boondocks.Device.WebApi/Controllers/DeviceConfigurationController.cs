using Boondocks.Base.Auth;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.Api.Resources;
using Boondocks.Device.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Messaging;
using NetFusion.Rest.Common;
using NetFusion.Rest.Server.Hal;
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
            ActionMeta(nameof(DeviceConfiguration)),
            Produces(typeof(DeviceConfigResource))]
        public async Task<IActionResult> GetConfiguration()
        {
            DeviceConfiguration config = await _messagingSrv.DispatchAsync(new GetDeviceConfiguration(_context.DeviceId));
            var configResource = new DeviceConfigResource {
                DeviceId = _context.DeviceId,
                RegistryName = config.Registry.RegistryName
            };

            configResource.Embed(AppVersionResource.FromVersionRef(config.Registry.ApplicationVersion), "app-version");
            configResource.Embed(AgentVersionResource.FromVersionRef(config.Registry.AgentVersion), "agent-version");

            return Ok(configResource);
        }

        [HttpGet("images/applications/{id}/download"), 
            ActionMeta(nameof(GetAppDownloadInfo)),
            Produces(typeof(AppVersionResource))]
        public async Task<IActionResult> GetAppDownloadInfo(Guid id)
        {
            IVersionReference versionRef = await _messagingSrv.DispatchAsync(new GetApplicationImageInfo(id));
            return Ok(AppVersionResource.FromVersionRef(versionRef));
        }

        #pragma warning disable CS4014
        [HttpGet(template: "images/agents/{id}/download"), 
            ActionMeta(nameof(GetAgentDownloadInfo)),
            Produces(typeof(AgentVersionResource))]
        public async Task<IActionResult> GetAgentDownloadInfo(Guid id)
        {
            IVersionReference versionRef = await _messagingSrv.DispatchAsync(new GetAgentImageInfo(id));
            return Ok(AgentVersionResource.FromVersionRef(versionRef));
        }

        public class DeviceConfigurationRelations : HalResourceMap
        {
            public override void OnBuildResourceMap()
            {
                Map<DeviceConfigResource>()
                    .LinkMeta<DeviceConfigurationController>(meta => {
                        meta.Url(RelationTypes.Self, (c, r) => c.GetConfiguration());
                    });

                Map<AppVersionResource>()
                    .LinkMeta<DeviceConfigurationController>(meta => {
                        meta.Url(RelationTypes.Self, (c, r) => c.GetAppDownloadInfo(r.Id));
                    });

                Map<AgentVersionResource>()
                    .LinkMeta<DeviceConfigurationController>(meta => {
                        meta.Url(RelationTypes.Self, (c, r) => c.GetAgentDownloadInfo(r.Id));
                    });
            }
        }

    }
}