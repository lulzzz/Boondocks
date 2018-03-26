using Boondocks.Auth.App;
using Boondocks.Auth.App.Providers;
using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NetFusion.Common.Extensions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        private IDeviceAuthRepository _deviceAuthRepo;

        public DeviceAuthProvider(
            ILogger<DeviceAuthProvider> logger,
            IDeviceAuthRepository deviceAuthRepo)
        {
            _logger = logger;
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
            
            var validation = await ValidateSignedDeviceToken(deviceToken);
            if (validation.authResult.IsAuthenticated)
            {
                return await GetResourceAccessClaimAsync(context, validation.deviceId);
            }

            return validation.authResult;
        }

        private async Task<(AuthResult authResult, Guid deviceId)> ValidateSignedDeviceToken(string deviceToken)
        {
            var token = new JwtSecurityToken(deviceToken);
            Guid deviceId = GetDeviceIdFromToken(token);

            if (deviceId == Guid.Empty)
            {
                return (AuthResult.Failed("Invalid credential token"), deviceId);
            }

            Guid? deviceKey = await _deviceAuthRepo.GetDeviceKeyAsync(deviceId);
            if (deviceKey == null)
            {
                _logger.LogDebug(
                    LogEvents.DeviceAuthKeyNotFound, 
                    "Device key not found for device with id: {deviceId}", deviceId);

                return (AuthResult.Failed("Invalid credential token"), deviceId);
            }

            try
            {
                var authResult = ValidateTokenUsingSymmetricKey(deviceKey.Value, deviceToken);
                return (authResult, deviceId);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(LogEvents.TokenValidationError, ex, 
                    "Error validating submitted device token {deviceToken} for Device Id: {deviceId}.", 
                    deviceToken, deviceId);

                return (AuthResult.Failed("Invalid credential token"), deviceId);

            }
            catch (Exception ex) {
                _logger.LogError(LogEvents.TokenValidationError, ex, 
                    "Unexpected error validating submitted device token {deviceToken} for Device Id: {deviceId}.", 
                    deviceToken, deviceId);
                throw;
            }
        }

        private Guid GetDeviceIdFromToken(JwtSecurityToken token)
        {
            var value = token.Claims.FirstOrDefault(c => c.Type == DeviceIdClaimName)?.Value;
            var deviceId = Guid.Empty;
            
            Guid.TryParse(value, out deviceId);    
            return deviceId;                    
        }

        // TODO:  Add support for multiple keys...
        private AuthResult ValidateTokenUsingSymmetricKey(Guid symmetricKey, string deviceToken)
        {
            TokenValidationParameters jwtValidationParams = ValidationParameters.Clone();

            jwtValidationParams.IssuerSigningKeys = new SecurityKey[] {
                new SymmetricSecurityKey(symmetricKey.ToByteArray())
                    {
                        KeyId = $"DeviceId_{symmetricKey:D}"
                    }
            };

            return AuthResult.SetAuthenticated(
                IsDeviceTokenValid(deviceToken, jwtValidationParams));
        }

        private bool IsDeviceTokenValid(string deviceToken, TokenValidationParameters validationParameters)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            
            handler.ValidateToken(deviceToken, validationParameters, 
                    out SecurityToken validatedToken);

            return validatedToken != null;
        }

        private async Task<AuthResult> GetResourceAccessClaimAsync(AuthContext context, Guid deviceId)
        { 
            ResourcePermission[] grantedAccessToResources = await _deviceAuthRepo.GetDeviceAccessAsync(deviceId, context.Resources);
            Claim accessClaim = new Claim("access", grantedAccessToResources.ToJson());

            return AuthResult.Authenticated(grantedAccessToResources, accessClaim);
        }
    }
}