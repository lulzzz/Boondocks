using System;
using System.Threading.Tasks;

namespace Boondocks.Base.Auth
{
    /// <summary>
    /// Repository for querying device authentication information.
    /// </summary>
    public interface IDeviceKeyAuthRepository
    {
        /// <summary>
        /// Returns the symmetric keys used to determine if the device token is correctly signed.
        /// </summary>
        /// <param name="deviceId">The identity of the device.</param>
        /// <returns>List of symmetric keys.</returns>
        Task<Guid?> GetDeviceKeyAsync(Guid deviceId);
    }
}