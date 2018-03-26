using Boondocks.Base.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Boondocks.Device.WebApi.Authentication
{
    public class DeviceAuthenticationHandler : AuthenticationHandler<DeviceAuthenticationOptions>
    {
        private const string AuthHeaderPrefix = "Bearer ";

        private IDeviceAuthService _deviceAuthSrv;

        public DeviceAuthenticationHandler(
            IDeviceAuthService deviceAuthService,
            IOptionsMonitor<DeviceAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock)
            
            : base(options, logger, encoder, clock)
        {
            _deviceAuthSrv = deviceAuthService;
        }
            
        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            string token = GetDeviceToken();
            if (token == null)
            {
                return AuthenticateResult.Fail("No valid authorization header was found.");
            }

            var validationParams = new TokenValidationParameters();
            validationParams.ValidIssuer = "boondocks-issuer";
            validationParams.ValidAudience = "boondocks-api";

            var validation = await _deviceAuthSrv.ValidateDeviceToken(token, validationParams);

            if (validation.authResult.IsAuthenticated)
            {
                var deviceIdentity = new DeviceIdentity(validation.deviceId, validation.authResult);
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
            // Get Authorization header value
            if (Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                var jwtToken = authorization.FirstOrDefault(v => v.StartsWith(AuthHeaderPrefix));
                if (jwtToken != null)
                {
                    return jwtToken.Remove(0, AuthHeaderPrefix.Length);
                }
            }
            return null;
        }
    }
}