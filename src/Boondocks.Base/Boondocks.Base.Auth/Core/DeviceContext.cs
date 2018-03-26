using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Boondocks.Base.Auth.Core
{
    /// <summary>
    /// Contains information about the device making the current request.
    /// </summary>
    public class DeviceContext : IDeviceContext
    {
        public Guid DeviceId { get; } = Guid.Empty;

        public DeviceContext(IHttpContextAccessor contextAccessor)
        {
            if (contextAccessor == null) throw new ArgumentNullException(nameof(contextAccessor));

            ClaimsPrincipal principal = contextAccessor.HttpContext.User;

            // If the principle has been authenticated the name corresponds to the DeviceId.
            if (principal != null && principal.Identity.IsAuthenticated)
            { 
                if (Guid.TryParse(principal.Identity.Name, out Guid deviceId))
                {
                    DeviceId = deviceId;
                }
            }
        }   
    }
}
