using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using Dapper;

namespace Boondocks.Device.Infra.Repositories
{
    /// <summary>
    /// Repository for managing device application logs.
    /// </summary>
    public class ApplicationLogRepository : IApplicationLogRepository
    {
        private readonly IRepositoryContext<DeviceDb> _context;
        
        public ApplicationLogRepository(IRepositoryContext<DeviceDb> context)
        {
            _context = context;
        }

        public async Task WriteDeviceLog(DeviceLog log, bool purgeBeforeWrite = false)
        {
           var connection = _context.OpenConn();

           using(var transaction = connection.BeginTransaction())
           {
                if (purgeBeforeWrite)
                {
                    await connection.ExecuteAsync(
                        "delete from ApplicationLogs where DeviceId = @deviceId", 
                        new {deviceId = log.DeviceId}, transaction);
                }

                const string sql = @"
                    insert into ApplicationLogs(Id, DeviceId, Type, Message, CreatedLocal, CreatedUtc) 
                    values (@Id, @DeviceId, @Type, @Message, @CreatedLocal, @CreatedUtc)";


                await connection.ExecuteAsync(sql, log.Entries, transaction);                
           }
        }
    }
}