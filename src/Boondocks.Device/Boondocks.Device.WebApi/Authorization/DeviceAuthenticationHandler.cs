using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Boondocks.Device.WebApi.Authentication
{
    public class DeviceAuthenticationHandler : AuthenticationHandler<DeviceAuthenticationOptions>
    {
        public DeviceAuthenticationHandler(IOptionsMonitor<DeviceAuthenticationOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }
            
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {


            return await Task.FromResult(
                    AuthenticateResult.Fail("No authorization header was found."));
;
        }

        private IEnumerable<SecurityKey> IssuerSigningKeyResolver(
            string token,
            SecurityToken securityToken,
            string kid,
            TokenValidationParameters validationParameters)
        {
            //TODO: implement multiple keys. That seems to be "good". MS does it. Other IoT identity providers do it.

          
                //Get the device key
                var deviceKey = Guid.NewGuid();

                //Return the security key(s) for this device.
                return new SecurityKey[]
                {
                    new SymmetricSecurityKey(deviceKey.ToByteArray())
                    {
                        KeyId = $"DeviceId_{deviceKey:D}"
                    }
                };
            
        }
    }
}