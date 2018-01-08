using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Boondocks.Services.Base;
using Boondocks.Services.Contracts;
using Boondocks.Services.DataAccess;
using Boondocks.Services.DataAccess.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Boondocks.Services.Device.WebApi.Authentication
{
    public class DeviceAuthenticationHandler : AuthenticationHandler<DeviceAuthenticationOptions>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private static readonly SecurityKey[] _emptySecurityKeys = { };
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        public DeviceAuthenticationHandler(IOptionsMonitor<DeviceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IDbConnectionFactory connectionFactory) 
            : base(options, logger, encoder, clock)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorization = Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorization))
            {
                return Task.FromResult(
                    AuthenticateResult.Fail("No authorization header was found."));
            }

            ClaimsPrincipal principal = _tokenHandler.ValidateToken(
                authorization,
                new TokenValidationParameters()
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
                },
                out SecurityToken _);

            //Get the device id
            string deviceId = principal.Claims
                .FirstOrDefault(c => c.Type == TokenConstants.DeviceIdClaimName)?.Value;
                
            var deviceIdentity = new ClaimsPrincipal(new DeviceIdentity(deviceId, true));

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
            var parsed = new JwtSecurityToken(token);

            //Get the device id
            Guid? deviceId = parsed.Claims
                .FirstOrDefault(c => c.Type == TokenConstants.DeviceIdClaimName)?.Value?
                .ParseGuid();

            if (deviceId == null)
                return _emptySecurityKeys;

            using (var connection = _connectionFactory.CreateAndOpen())
            {
                //Get the device key
                Guid? deviceKey = connection.GetDeviceKey(deviceId.Value);

                //Make sure we got it back.
                if (deviceKey == null)
                    return _emptySecurityKeys;

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