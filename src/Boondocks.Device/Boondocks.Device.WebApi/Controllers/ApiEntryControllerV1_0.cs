using System;
using System.Threading.Tasks;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetFusion.Rest.Resources.Hal;
using NetFusion.Rest.Server.Hal;

namespace Boondocks.Device.WebApi.Controllers
{
    /// <summary>
    /// Exposes a resource containing the entry-point API URIs for the Microservice.
    /// This allows clients requesting HAL to resource responses to fine the entry
    /// URIs they can use to start communication with service.
    /// </summary>
    [Route("v1.0/device/entries")]
    [AllowAnonymous]
    public class ApiEntryControllerV1_0 : Controller
    {
        [AllowAnonymous, HttpGet]
        public ApiEntryResourceV1_0 GetEntryPoint() 
        {
            return new ApiEntryResourceV1_0 {};
        }
    }

    /// <summary>
    /// Resource containing the entry-point URIs for the microservice.
    /// </summary>
    public class ApiEntryResourceV1_0 : HalEntryPointResource
    {
        public ApiEntryResourceV1_0()
        {
            Version = typeof(ApiEntryResourceV1_0).Assembly
                .GetName().Version.ToString();   
        }
    }

    public class ApiEntryPointRelations : HalResourceMap
    {
        // Adds HAL links to the entry service API methods from which communication
        // can start with the service.
        public override void OnBuildResourceMap()
        {
            Map<ApiEntryResourceV1_0>()
                .LinkMeta<DeviceConfigurationController>(meta => {
                    meta.UrlTemplate<Guid, Task<IActionResult>>("appInfo", c => c.GetAppDownloadInfo);
                    meta.UrlTemplate<Guid, Task<IActionResult>>("agentInfo", c => c.GetAppDownloadInfo);
                    meta.UrlTemplate<Task<DeviceConfiguration>>("deviceConfigInfo", c => c.GetConfiguration);
                })
                .LinkMeta<HeartbeatController>(meta => {
                    meta.UrlTemplate<DeviceHeartbeatModel, Task<HeartbeatResponseModel>>("recordHeartbeat", c => c.RecordHeartbeat);
                })
                .LinkMeta<LogEventController>(meta => {
                    meta.UrlTemplate<LogEventsModel, Task>("recordLogEvent", c => c.RecordLogEvent);
                    meta.UrlTemplate<LogEventsModel, Task>("purgeAndRecordLogEvent", c => c.PurgeAndRecordLogEvent);
                });
        }
    }
}