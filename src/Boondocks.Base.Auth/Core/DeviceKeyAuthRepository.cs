using System;
using System.Threading.Tasks;
using Boondocks.Base.Data;
using Dapper;

namespace Boondocks.Base.Auth.Core
{
    /// <summary>
    /// Default repository implementation that resolves the DeviceKey for a specified DeviceId.
    /// </summary>
    public class DeviceKeyAuthRepository : IDeviceKeyAuthRepository
    {
        private readonly IRepositoryContext<AuthDb> _context;

        public DeviceKeyAuthRepository(IRepositoryContext<AuthDb> context)
        {
            _context = context;
        }

        public Task<Guid?> GetDeviceKeyAsync(Guid deviceId)
        {
            if (deviceId == null || deviceId == Guid.Empty) 
            {
                throw new ArgumentException("Device id not specified.", nameof(deviceId));
            }

            const string deviceSql = @"
                SELECT 
                    DeviceKey
                FROM dbo.Devices 
                WHERE Id = @deviceId";

            return _context.OpenConn()
                .QueryFirstOrDefaultAsync<Guid?>(deviceSql, new { deviceId });
        }
    }
}