using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Boondocks.Base;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Boondocks.Services.Device.WebApi.Authentication
{
    /// <summary>
    /// Implements our custom authentication for devices.
    /// </summary>
    public class DeviceAuthenticationHandler : AuthenticationHandler<DeviceAuthenticationOptions>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private static readonly SecurityKey[] _emptySecurityKeys = { };
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
        private readonly TokenValidationParameters _tokenValidationParameters;

        public DeviceAuthenticationHandler(IOptionsMonitor<DeviceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IDbConnectionFactory connectionFactory) 
            : base(options, logger, encoder, clock)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

            _tokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKeyResolver = IssuerSigningKeyResolver,
                ValidAudiences = new[]
                {
                    TokenConstants.DeviceTokenAudience
                },
                ValidIssuers = new[]
                {
                    TokenConstants.DeviceTokenIssuer
                },
            };
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //Get the value from the header
            string authorization = Request.Headers["Authorization"];

            //Make sure we got a value
            if (string.IsNullOrWhiteSpace(authorization))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("No authorization header was found."));
            }

            //Attempt to validate the token
            ClaimsPrincipal principal = _tokenHandler.ValidateToken(
                authorization,
                _tokenValidationParameters,
                out SecurityToken _);

            //Get the device id
            string deviceId = principal.Claims
                .FirstOrDefault(c => c.Type == TokenConstants.DeviceIdClaimName)?.Value;
                
            //Create an identity
            var deviceIdentity = new ClaimsPrincipal(new DeviceIdentity(deviceId, true));

            //Return it.
            return Task.FromResult(
                    AuthenticateResult.Success(
                        new AuthenticationTicket(
                            deviceIdentity,
                            new AuthenticationProperties(),
                            "Device")));
        }

        /// <summary>
        /// This gets the correct key for the device.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="securityToken"></param>
        /// <param name="kid"></param>
        /// <param name="validationParameters"></param>
        /// <returns></returns>
        private IEnumerable<SecurityKey> IssuerSigningKeyResolver(
            string token, 
            SecurityToken securityToken,
            string kid, 
            TokenValidationParameters validationParameters)
        {
            //TODO: implement multiple keys. That seems to be "good". MS does it. Other IoT identity providers do it.

            //Parse the token
            var parsed = new JwtSecurityToken(token);

            //Get the device id
            Guid? deviceId = parsed.Claims
                .FirstOrDefault(c => c.Type == TokenConstants.DeviceIdClaimName)?.Value?
                .TryParseGuid();

            //Check to see if we got a deviceId.
            //TODO: Determin if there is a more informative exception that we could throw.
            if (deviceId == null)
                return _emptySecurityKeys;

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                //Get the device key
                Guid? deviceKey = connection.GetDeviceKey(deviceId.Value);

                //Make sure we got it back.
                if (deviceKey == null)
                    return _emptySecurityKeys;

                //Return the security key(s) for this device.
                return new SecurityKey[]
                {
                    new SymmetricSecurityKey(deviceKey.Value.ToByteArray())
                    {
                        KeyId = $"DeviceId_{deviceId:D}"
                    },
                };
            }
        }
    }
}