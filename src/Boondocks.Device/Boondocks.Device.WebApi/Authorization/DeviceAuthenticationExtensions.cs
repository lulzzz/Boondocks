using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Boondocks.Device.WebApi.Authentication
{
    public static class DeviceAuthenticationExtensions
    {
        public static AuthenticationBuilder AddDeviceAuthentication<TAuthService>(this AuthenticationBuilder builder, 
            string authenticationScheme, 
            Action<DeviceAuthenticationOptions> configureOptions)
        where TAuthService : class, IDeviceAuthenticationService
    {
        builder.Services.AddSingleton<DeviceAuthenticationOptions>();
        builder.Services.AddTransient<IDeviceAuthenticationService, TAuthService>();

        return builder.AddScheme<DeviceAuthenticationOptions, DeviceAuthenticationHandler>(
            authenticationScheme, configureOptions);
    }
    }
}