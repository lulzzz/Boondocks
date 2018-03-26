using Boondocks.Auth.App.Modules;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Services;
using Boondocks.Auth.Infra.Configs;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Boondocks.Auth.Infra
{
    /// <summary>
    /// Infrastructure service responsible for generating a signed JTW token.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IAuthCertificateModule _certificateModule;
        private readonly JwtTokenSettings _tokenConfig;
        
        public TokenService(
            IAuthCertificateModule certificateModule,
            JwtTokenSettings tokenConfig)
        {
            _certificateModule = certificateModule;
            _tokenConfig = tokenConfig;
        }

        public string CreateClaimToken(ResourcePermission[] resourcePermissions)
        {
            X509SecurityKey privateKey = _certificateModule.GetPrivateKey();
            
            var signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
            var jwtHeader = new JwtHeader(signingCredentials);
            var tokenDates = GetTokenDates();

            var tokenPayload = new JwtPayload(_tokenConfig.Issuer, _tokenConfig.Audience,
                new Claim[]{},
                tokenDates.nbf,
                tokenDates.exp,
                tokenDates.iat);

            tokenPayload["access"] = resourcePermissions;

            var securityToken = new JwtSecurityToken(jwtHeader, tokenPayload);
            var tokenHandler = new JwtSecurityTokenHandler();

            string signedToken = tokenHandler.WriteToken(securityToken);
            return signedToken;
        }

        public (DateTime iat, DateTime nbf, DateTime exp) GetTokenDates()
        {
            DateTime now = DateTime.UtcNow;

            return (
                now, 
                now.AddMinutes(-30), 
                now.AddMinutes(_tokenConfig.ValidForMinutes));
        }
    }
}