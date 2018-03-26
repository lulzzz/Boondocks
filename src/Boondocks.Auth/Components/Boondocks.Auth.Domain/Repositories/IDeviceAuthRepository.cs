using Boondocks.Auth.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Boondocks.Auth.Domain.Repositories
{
    /// <summary>
    /// Repository for querying device authentication information.
    /// </summary>
    public interface IDeviceAuthRepository
    {
        /// <summary>
        /// For an authenticated device, return a list of registry access items that the user has access.
        /// </summary>
        /// <param name="deviceId">The key value of the device.</param>
        /// <param name="resourceAccess">An array of registry resources for which access is being requested.</param>
        /// <returns>A subset of the requested access registry items to which the device has access.</returns>
        Task<ResourcePermission[]> GetDeviceAccessAsync(Guid deviceId, ResourcePermission[] resourceAccess);
    }
}