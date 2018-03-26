using Boondocks.Auth.App;
using Boondocks.Auth.App.Providers;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Repositories;
using Boondocks.Base.Auth;
using Microsoft.Extensions.Logging;
using NetFusion.Common.Extensions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Boondocks.Auth.Infra
{
    /// <summary>
    /// Authenticates a device identified by a singed JWT token. 
    /// </summary>
    public class DeviceAuthProvider : AuthProviderBase
    {
        private const string DeviceTokenPropertyName = "device-token";
        private const string DeviceIdClaimName = "device-id";

        private ILogger<DeviceAuthProvider> _logger;
        private IDeviceAuthService _deviceAuthSrv;
        private IDeviceAuthRepository _deviceAuthRepo;

        public DeviceAuthProvider(
            ILogger<DeviceAuthProvider> logger,
            IDeviceAuthService deviceAuthSrv,
            IDeviceAuthRepository deviceAuthRepo)
        {
            _logger = logger;
            _deviceAuthSrv = deviceAuthSrv;
            _deviceAuthRepo = deviceAuthRepo;
        }

        /// <summary>
        /// Determines if the submitted authentication context and credentials contain
        /// the information needed to validate a device.
        /// </summary>
        public override bool IsValidAuthenticationRequest(AuthContext context)
        {
            var credentials = context.Credentials;

            if (credentials.Keys.Count != 1)
            {
                _logger.LogError(LogEvents.RequiredCredentialsError, "Invalid number of credentials received.");
                return false;
            }

            if (! credentials.TryGetValue(DeviceTokenPropertyName, out string deviceToken) 
                || String.IsNullOrWhiteSpace(deviceToken))
            {
                _logger.LogError(LogEvents.RequiredCredentialsError, 
                    $"Credentials does not contain valid property named: {DeviceTokenPropertyName}");

                return false;
            }

            return true;
        }

        public override async Task<AuthResult> OnAuthenticateAsync(AuthContext context)
        {
            string deviceToken = context.Credentials[DeviceTokenPropertyName];
            
            var validation = await _deviceAuthSrv.ValidateDeviceToken(deviceToken, ValidationParameters);
            if (validation.authResult.IsAuthenticated)
            {
                return await GetResourceAccessClaimAsync(context, validation.deviceId);
            }

            if (validation.authResult.IsAuthenticated)
            {
                return AuthResult.Authenticated();
            }

            return AuthResult.Failed(validation.authResult.Reason);
        }

        private async Task<AuthResult> GetResourceAccessClaimAsync(AuthContext context, Guid deviceId)
        { 
            ResourcePermission[] grantedAccessToResources = await _deviceAuthRepo.GetDeviceAccessAsync(deviceId, context.Resources);
            Claim accessClaim = new Claim("access", grantedAccessToResources.ToJson());

            return AuthResult.Authenticated(grantedAccessToResources, accessClaim);
        }
    }
}