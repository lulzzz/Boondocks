using System;
using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using Dapper;

namespace Boondocks.Device.Infra.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IRepositoryContext<DeviceDb> _context;
    
        public DeviceRepository(IRepositoryContext<DeviceDb> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<DeviceVersion> GetDeviceVersionInfo(Guid deviceId)
        {
            const string responseSql = "select ConfigurationVersion from Devices where Id = @Id";
            return _context.OpenConn().
                QuerySingleAsync<DeviceVersion>(responseSql, new {Id = deviceId});
        }

        public Task UpdateDeviceStatus(DeviceStatus heartbeat)
        {
            const string updateSql = @"
                update DeviceStatus 
                    set 
                        AgentVersion = @AgentVersion, 
                        ApplicationVersion = @ApplicationVersion, 
                        UptimeSeconds = @UptimeSeconds, 
                        LastContactUtc = @LastContactUtc, 
                        State = @State 
                    where 
                        DeviceId = @DeviceId";

            return _context.OpenConn()
                .ExecuteAsync(updateSql, heartbeat);
        }
    }
}