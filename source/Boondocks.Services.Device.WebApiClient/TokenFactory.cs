namespace Boondocks.Services.Device.WebApiClient
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Microsoft.IdentityModel.Tokens;
    using Services.Contracts;

    /// <summary>
    /// Used to create Authorization tokens.
    /// </summary>
    internal class TokenFactory
    {
        private readonly ClaimsIdentity _claimsIdentity;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public TokenFactory(Guid deviceId, Guid deviceKey)
        {
            //Create the claims
            var claims = new[]
            {
                new Claim(TokenConstants.DeviceIdClaimName, deviceId.ToString("D"))
            };

            //Create the claims identity
            _claimsIdentity = new ClaimsIdentity(claims);

            //Create the signing credentials
            _signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(deviceKey.ToByteArray()),
                SecurityAlgorithms.HmacSha256Signature);

            _tokenHandler = new JwtSecurityTokenHandler();
        }

        internal string CreateToken()
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = _claimsIdentity,

                Issuer = TokenConstants.DeviceTokenIssuer,
                Audience = TokenConstants.DeviceTokenAudience,

                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                SigningCredentials = _signingCredentials
            };

            var securityToken = _tokenHandler.CreateToken(tokenDescriptor);

            return _tokenHandler.WriteToken(securityToken);
        }
    }
}