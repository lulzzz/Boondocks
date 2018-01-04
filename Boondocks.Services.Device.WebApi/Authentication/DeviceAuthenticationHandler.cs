using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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

            string deviceId = usernamePassword.Substring(0, seperatorIndex);
            string deviceKeyRaw = usernamePassword.Substring(seperatorIndex + 1);

            if (Guid.TryParse(deviceKeyRaw, out Guid deviceKey))
            {
                //TODO: Verify the devicekey / password

                return Task.FromResult(
                    AuthenticateResult.Success(
                        new AuthenticationTicket(
                            new ClaimsPrincipal(new DeviceIdentity(deviceId, true)),
                            new AuthenticationProperties(),
                            "Bearer")));
            }

            return Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(new DeviceIdentity(deviceId, false)),
                        new AuthenticationProperties(),
                        "Bearer")));


        }
    }
}