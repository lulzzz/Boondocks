using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boondocks.Device.Domain.Repositories
{
    public interface IHealthCheckRepository 
    {
        Task<IDictionary<string, DateTime?>> GetDatabaseStatus();
    }
}