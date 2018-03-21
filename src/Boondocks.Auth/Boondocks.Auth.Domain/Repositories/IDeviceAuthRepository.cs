using System;
using System.Threading.Tasks;
using Boondocks.Auth.Domain.Entities;

namespace Boondocks.Auth.Domain.Repositories
{
    /// <summary>
    /// Repository for querying device authentication information.
    /// </summary>
    public interface IDeviceAuthRepository
    {
        /// <summary>
        /// Returns the symmetric keys used to determine if the device token is correctly signed.
        /// </summary>
        /// <param name="deviceId">The identity of the device.</param>
        /// <returns>List of symmetric keys.</returns>
        Task<Guid?> GetDeviceKeyAsync(Guid deviceId);

        /// <summary>
        /// For an authenticated device, return a list of registry access items that the user has access.
        /// </summary>
        /// <param name="deviceId">The key value of the device.</param>
        /// <param name="resourceAccess">An array of registry resources for which access is being requested.</param>
        /// <returns>A subset of the requested access registry items to which the device has access.</returns>
        Task<ResourcePermission[]> GetDeviceAccessAsync(Guid deviceId, ResourcePermission[] resourceAccess);
    }
}