using Boondocks.Base.Data;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Repositories;
using Boondocks.Device.Infra.Repositories.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boondocks.Device.Infra.Repositories
{
    public class HealthCheckRepository : IHealthCheckRepository
    {
        private readonly IRepositoryContext<DeviceDb> _context;

        public HealthCheckRepository(IRepositoryContext<DeviceDb> context)
        {
            _context = context;
        }

        public async Task<IDictionary<string, DateTime?>> GetDatabaseStatus()
        {
           const string statusSql = @"
            select 'DeviceStatus' Name, max(LastContactUtc) Value from dbo.DeviceStatus
            union
            select 'ApplicationEvents' Name, max(CreatedUtc) Value from dbo.ApplicationEvents
            union
            select 'ApplicationLogs' Name, max(CreatedUtc) Value  from dbo.ApplicationLogs
            union
            select 'DeviceEvents' Name, max(CreatedUtc) Value from dbo.DeviceEvents";

            return ( await _context.OpenConn()
                .QueryAsync<NamedValueData<DateTime?>>(statusSql))
                .ToDictionary(row => row.Name, row => row.Value);
        }
    }
}