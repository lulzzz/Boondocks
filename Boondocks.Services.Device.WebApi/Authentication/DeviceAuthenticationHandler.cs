using System;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using Boondocks.Services.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Boondocks.Services.Device.WebApi.Authentication
{
    public class DeviceAuthenticationHandler : AuthenticationHandler<DeviceAuthenticationOptions>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DeviceAuthenticationHandler(IOptionsMonitor<DeviceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IDbConnectionFactory connectionFactory) 
            : base(options, logger, encoder, clock)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            const string Basic = "Basic ";

            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith(Basic))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("No authorization header was found."));
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
                AuthenticateResult.Fail("Unable to find device id / key"));


        }
    }
}