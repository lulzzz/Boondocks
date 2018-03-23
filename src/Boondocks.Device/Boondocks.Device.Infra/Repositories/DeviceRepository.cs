using System;
using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Boondocks.Device.Infra.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IRepositoryContext<DeviceDb> _context;
    
        public DeviceRepository(IRepositoryContext<DeviceDb> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DeviceEntity> GetDevice(Guid deviceId)
        {
            const string deviceSql = @"
                SELECT 
                    Id, 
                    Name, 
                    DeviceKey, 
                    ApplicationId, 
                    ApplicationVersionId, 
                    AgentVersionId, 
                    RootFileSystemVersionId,     
                    ConfigurationVersion, 
                    CreatedUtc
                FROM dbo.Devices 
                WHERE Id = @deviceId";
            var device = await _context.OpenConn()
                .QueryFirstAsync<DeviceEntity>(deviceSql, new { deviceId });

            const string variableSql = "select * from DeviceEnvironmentVariables where DeviceId = @deviceId";

            var variables = await _context.OpenConn()
                .QueryAsync<EnvironmentVariable>(variableSql, new { deviceId });

            device.SetVariables(variables);
            return device;
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