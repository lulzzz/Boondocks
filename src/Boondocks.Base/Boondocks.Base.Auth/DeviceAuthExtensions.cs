using Boondocks.Base.Auth.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace Boondocks.Base.Auth
{
    public static class DeviceAuthExtensions
    {
        /// <summary>
        /// Adds device authentication handler.
        /// </summary>
        /// <param name="builder">The authentication build being configured.</param>
        /// <param name="options">The options used as validation parameters.</param>
        /// <returns>Configured authentication builder.</returns>
        public static AuthenticationBuilder AddDeviceTokenAuth(this AuthenticationBuilder builder, 
            DeviceAuthOptions options)
        {
            return builder.AddScheme<DeviceAuthOptions, DeviceAuthHandler>(
                JwtBearerDefaults.AuthenticationScheme, (authOptions) => {

                    authOptions.Issuer = options.Issuer;
                    authOptions.Audience = options.Audience;
                });
        }

        /// <summary>
        /// Returns the JWT validation parameter authentication options for the host's configuration.
        /// </summary>
        /// <param name="configuration">The application's configuration.</param>
        /// <returns>Populated configuration object instance.</returns>
        public static DeviceAuthOptions GetDeviceOptions(this IConfiguration configuration)
        {
            var deviceOptions = new DeviceAuthOptions
            {
                Issuer = "boondocks-issuer",
                Audience = "boondocks-api"
            };
            configuration.GetSection("boondocks:auth:device").Bind(deviceOptions);
            return deviceOptions;
        }
    }
}
