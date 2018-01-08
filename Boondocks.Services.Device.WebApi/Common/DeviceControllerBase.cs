using System;
using Boondocks.Services.Base;
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
            get
            {
                Guid? deviceId = HttpContext.User.Identity.Name.ParseGuid();

                if (deviceId == null)
                    throw new InvalidOperationException("Unable to find deviceId.");

                return deviceId.Value;
            }
        }
    }
}