using Boondocks.Base.Auth;
using System;
using System.Threading.Tasks;

namespace Boondocks.Auth.Infra.Repositories
{
    public class DeviceKeyAuthRepository : IDeviceKeyAuthRepository
    {
        public Task<Guid?> GetDeviceKeyAsync(Guid deviceId)
        {
            Guid? deviceKey = Guid.Parse("671674D6-7D14-4DC0-94A0-B1085B878C23"); // TODO:  Provider real implementation.
            return Task.FromResult(deviceKey);
        }
    }
}