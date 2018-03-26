using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Boondocks.Auth.Infra.Repositories
{
    public class DeviceAuthRepository : IDeviceAuthRepository
    {
        public Task<ResourcePermission[]> GetDeviceAccessAsync(Guid deviceId, ResourcePermission[] resourceAccess)
        {
            return Task.FromResult(resourceAccess); // TODO: determine base on device registry access logic.
        }
    }
}