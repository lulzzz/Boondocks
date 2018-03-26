using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Boondocks.Base.Auth.Core
{
    public class DeviceContext : IDeviceContext
    {
        public Guid DeviceId { get; } = Guid.Empty;

        public DeviceContext(IHttpContextAccessor contextAccessor)
        {
            ClaimsPrincipal principal = contextAccessor.HttpContext.User;

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
