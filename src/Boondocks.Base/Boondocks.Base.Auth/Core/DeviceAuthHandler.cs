using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Boondocks.Base.Auth.Core
{
    /// <summary>
    /// Handler that authenticates a JWT device token when registered as part of the 
    /// HTTP pipeline.  
    /// </summary>
    public class DeviceAuthHandler : AuthenticationHandler<DeviceAuthOptions>
    {
        private IDeviceAuthService _deviceAuthSrv;

        public DeviceAuthHandler(
            IDeviceAuthService deviceAuthService,
            IOptionsMonitor<DeviceAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)

            : base(options, logger, encoder, clock)
        {
            _deviceAuthSrv = deviceAuthService ?? throw new ArgumentNullException(nameof(deviceAuthService));
        }

        /// <summary>
        /// Reads the JWT Bearer token from the HTTP header.  If present, the token containing the signed
        /// device-token is validated.
        /// </summary>
        /// <returns></returns>
        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string token = GetDeviceToken();
            if (token == null)
            {
                return AuthenticateResult.Fail("Valid authorization header not found.");
            }

            // Create validation parameters based on provided configuration.
            var validationParams = new TokenValidationParameters
            {
                ValidIssuer = Options.Issuer,
                ValidAudience = Options.Audience
            };

            // Delegate to service containing the common device-authentication logic.
            var validation = await _deviceAuthSrv.ValidateDeviceToken(token, validationParams);

            // Create a DeviceIdentity from the validation results.
            var deviceIdentity = new DeviceIdentity(validation.deviceId, validation.authResult);

            if (deviceIdentity.IsAuthenticated)
            {
                // If the device identity was authenticated, create a claims principal and return as 
                // part of the authentication ticket.  This will set the principal on the current thread.
                var principal = new ClaimsPrincipal(deviceIdentity);

                return AuthenticateResult.Success(
                        new AuthenticationTicket(
                            principal,
                            new AuthenticationProperties(),
                            "Device"));
            }

            return AuthenticateResult.Fail(validation.authResult.Reason);
        }

        private string GetDeviceToken()
        {
            if (Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                var jwtToken = authorization.FirstOrDefault(v => v.StartsWith(JwtBearerDefaults.AuthenticationScheme));
                if (jwtToken != null)
                {
                    return jwtToken.Remove(0, JwtBearerDefaults.AuthenticationScheme.Length+1);
                }
            }
            return null;
        }
    }
}