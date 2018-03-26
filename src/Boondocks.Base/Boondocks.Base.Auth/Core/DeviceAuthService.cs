using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Boondocks.Base.Auth.Core
{
    public class DeviceAuthService : IDeviceAuthService
    {
        private const string DeviceIdClaimName = "device-id";

        private ILogger<DeviceAuthService> _logger;
        private IDeviceKeyAuthRepository _deviceKeyAuthRepo;

        public DeviceAuthService(
            ILogger<DeviceAuthService> logger,
            IDeviceKeyAuthRepository deviceKeyAuthRepo)
        {
            _logger = logger;
            _deviceKeyAuthRepo = deviceKeyAuthRepo;

        }

        public async Task<(KeyAuthResult authResult, Guid deviceId)> ValidateDeviceToken(string deviceToken, TokenValidationParameters validationParams)
        {           
            var token = new JwtSecurityToken(deviceToken);
            Guid deviceId = GetDeviceIdFromToken(token);

            if (deviceId == Guid.Empty)
            {
                return (KeyAuthResult.Failed("Invalid credential token"), deviceId);
            }

            Guid? deviceKey = await _deviceKeyAuthRepo.GetDeviceKeyAsync(deviceId);
            if (deviceKey == null)
            {
                _logger.LogDebug("Device key not found for device with id: {deviceId}", deviceId);

                return (KeyAuthResult.Failed("Invalid credential token"), deviceId);
            }

            try
            {
                var authResult = ValidateTokenUsingSymmetricKey(deviceKey.Value, deviceToken, validationParams);
                return (authResult, deviceId);
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Error validating submitted device token {deviceToken} for Device Id: {deviceId}.",
                    deviceToken, deviceId);

                return (KeyAuthResult.Failed("Invalid credential token"), deviceId);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error validating submitted device token {deviceToken} for Device Id: {deviceId}.",
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
        private KeyAuthResult ValidateTokenUsingSymmetricKey(Guid symmetricKey, string deviceToken, TokenValidationParameters validationParams)
        {
            TokenValidationParameters jwtValidationParams = validationParams.Clone();

            jwtValidationParams.IssuerSigningKeys = new SecurityKey[] {
                new SymmetricSecurityKey(symmetricKey.ToByteArray())
                    {
                        KeyId = $"DeviceId_{symmetricKey:D}"
                    }
            };

            return KeyAuthResult.SetAuthenticated(
                IsDeviceTokenValid(deviceToken, jwtValidationParams));
        }

        private bool IsDeviceTokenValid(string deviceToken, TokenValidationParameters validationParameters)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            handler.ValidateToken(deviceToken, validationParameters,
                    out SecurityToken validatedToken);

            return validatedToken != null;
        }
    }
}
