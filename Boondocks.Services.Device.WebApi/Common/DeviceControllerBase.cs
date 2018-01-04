using System;
using Boondocks.Services.Device.WebApi.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Boondocks.Services.Device.WebApi.Common
{
    public abstract class DeviceControllerBase : Controller
    {
        /// <summary>
        /// Get the id of the device.
        /// </summary>
        public Guid DeviceId
        {
            get { return ((DeviceIdentity) HttpContext.User.Identity).DeviceId; }
        }
    }
}