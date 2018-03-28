using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Boondocks.Base.Auth
{
    /// <summary>
    /// Service for authenticating a device token signed using the Device-Key associated with
    /// the DeviceId.
    /// </summary>
    public  interface IDeviceAuthService
    {
        /// <summary>
        /// Validates a signed device token.
        /// </summary>
        /// <param name="deviceToken">The signed device token to be validated.</param>
        /// <param name="validationParams">The validation parameters to be used for validation.</param>
        /// <returns>The results of the authentication and the DeviceId.  If the token was not
        /// validated, DeviceId is set to GUID.Empty value.</returns>
        Task<(DeviceAuthResult authResult, Guid deviceId)> ValidateDeviceToken(string deviceToken, 
            TokenValidationParameters validationParams);
    }
}
