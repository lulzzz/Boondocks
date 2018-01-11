﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Boondocks.Services.Contracts;
using Boondocks.Services.Device.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Boondocks.Services.Device.WebApiClient
{
    public class DeviceApiClient : CaptiveAire.WebApiClient.WebApiClient
    {
        private readonly ClaimsIdentity _claimsIdentity;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        private static class ResourceUris
        {
            public const string Heartbeat = "v1/heartbeat";
            public const string ApplicationVersions = "v1/applicationVersions";
            public const string DeviceConfiguration = "v1/deviceConfiguration";
        }

        public DeviceApiClient(Guid deviceId, Guid deviceKey, string baseUri)
        {
            BaseUri = baseUri;

            //Create the claims
            var claims = new Claim[]
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

        protected override Task<HttpClient> CreateHttpClientAsync()
        {
            var client = new HttpClient();

            string token = CreateToken();
            
            //Add the authorization header
            client.DefaultRequestHeaders.Add("Authorization", token);

            return Task.FromResult(client);
        }

        protected virtual string CreateToken()
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = _claimsIdentity,

                Issuer = TokenConstants.DeviceTokenIssuer,
                Audience = TokenConstants.DeviceTokenAudience,

                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                SigningCredentials = _signingCredentials
            };

            SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);

            return _tokenHandler.WriteToken(securityToken);
        }

        protected override string BaseUri { get; }

        /// <summary>
        /// Heartbeat.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HeartbeatResponse> HeartbeatAsync(HeartbeatRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            return PostAsync<HeartbeatResponse>(ResourceUris.Heartbeat, request, null, cancellationToken);
        }

        /// <summary>
        /// Gets the configuration for the device.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<GetDeviceConfigurationResponse> GetConfigurationAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return GetAsync<GetDeviceConfigurationResponse>(ResourceUris.DeviceConfiguration, null, cancellationToken);
        }

        /// <summary>
        /// Gets the download stream for an application image.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Stream> DownloadApplicationVersionImage(Guid id, CancellationToken cancellationToken = new CancellationToken())
        {
            return DownloadFileAsync(ResourceUris.ApplicationVersions, new { id }, cancellationToken);
        }
    }
}