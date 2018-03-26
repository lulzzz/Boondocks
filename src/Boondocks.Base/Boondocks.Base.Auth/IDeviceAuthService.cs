using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace Boondocks.Base.Auth
{
    public  interface IDeviceAuthService
    {
        Task<(KeyAuthResult authResult, Guid deviceId)> ValidateDeviceToken(string deviceToken, TokenValidationParameters validationParams);
    }
}
