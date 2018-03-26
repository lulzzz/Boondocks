using Boondocks.Base.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Boondocks.Device.WebApi.Authentication
{
    public static class DeviceAuthenticationExtensions
    {
        public static AuthenticationBuilder AddDeviceAuthentication<TAuthService>(this AuthenticationBuilder builder, 
            string authenticationScheme, 
            Action<DeviceAuthenticationOptions> configureOptions)
        where TAuthService : class, IDeviceAuthService
    {
        builder.Services.AddSingleton<DeviceAuthenticationOptions>();
        builder.Services.AddTransient<IDeviceAuthService, TAuthService>();

        return builder.AddScheme<DeviceAuthenticationOptions, DeviceAuthenticationHandler>(
            authenticationScheme, configureOptions);
    }
    }
}