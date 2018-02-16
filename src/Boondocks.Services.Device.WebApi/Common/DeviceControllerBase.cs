namespace Boondocks.Services.Device.WebApi.Common
{
    using System;
    using Base;
    using Microsoft.AspNetCore.Mvc;

    public abstract class DeviceControllerBase : Controller
    {
        /// <summary>
        ///     Get the id of the device.
        /// </summary>
        public Guid DeviceId
        {
            get
            {
                var deviceId = HttpContext.User.Identity.Name.TryParseGuid();

                if (deviceId == null)
                    throw new InvalidOperationException("Unable to find deviceId.");

                return deviceId.Value;
            }
        }
    }
}