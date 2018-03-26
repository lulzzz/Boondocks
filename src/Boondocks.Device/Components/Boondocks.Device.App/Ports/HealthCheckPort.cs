using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain;
using Boondocks.Device.Domain.Repositories;
using NetFusion.Messaging;

namespace Boondocks.Device.App.Ports
{
    /// <summary>
    /// Handles a query requesting information used to determine the execution
    /// health of the service.  Responds with the latest maximum date the sate
    /// of certain entities were updated.
    /// </summary>
    public class HealthCheckPort : IQueryConsumer
    {
        private readonly IRepositoryContext<DeviceDb> _repoContext;
        private IHealthCheckRepository _healthCheckRepo;

        public HealthCheckPort(
            IRepositoryContext<DeviceDb> repoContext,
            IHealthCheckRepository healthCheckRepo)
        {
            _repoContext = repoContext;
            _healthCheckRepo = healthCheckRepo;
        }

        public async Task<ServiceStatus> ForQuery(GetHealthCheckStatus query)
        {
            IDictionary<string, DateTime?> dbUpdateStatuses;
            
            using (_repoContext)
            {
                dbUpdateStatuses = await _healthCheckRepo.GetDatabaseStatus();
            }
            
            // Return service status containing dictionary with the dates the 
            // last time certain information was updated.  Additional information
            // can be added to this status class if needed.
            return new ServiceStatus(dbUpdateStatuses);
        }
    }
}