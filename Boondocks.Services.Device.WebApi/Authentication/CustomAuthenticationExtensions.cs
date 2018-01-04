using System;
using Microsoft.AspNetCore.Authentication;

namespace Boondocks.Services.Device.WebApi.Authentication
{
    public static class CustomAuthenticationExtensions
    {
        public static AuthenticationBuilder AddCustomAuthentication(this AuthenticationBuilder builder,
            string authenticationScheme, string displayName, Action<DeviceAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<DeviceAuthenticationOptions, DeviceAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}