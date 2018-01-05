using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Boondocks.Services.Device.WebApi.Authentication
{
    public class DeviceAuthenticationHandler : AuthenticationHandler<DeviceAuthenticationOptions>
    {
        public DeviceAuthenticationHandler(IOptionsMonitor<DeviceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            const string Basic = "Basic ";

            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith(Basic))
            {
                return null;
            }

            string encodedUsernamePassword = authorization.Substring(Basic.Length);

            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            Guid? deviceId = usernamePassword.Substring(0, seperatorIndex).ParseGuid();
            Guid? deviceKey = usernamePassword.Substring(seperatorIndex + 1).ParseGuid();

            if (deviceId != null && deviceKey != null)
            {
                //TODO: Verify the devicekey / password

                return Task.FromResult(
                    AuthenticateResult.Success(
                        new AuthenticationTicket(
                            new ClaimsPrincipal(new DeviceIdentity(deviceId.Value, true)),
                            new AuthenticationProperties(),
                            "Bearer")));
            }

            return Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(new DeviceIdentity(deviceId ?? Guid.Empty, false)),
                        new AuthenticationProperties(),
                        "Bearer")));


        }
    }
}