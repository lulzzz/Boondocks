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
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IRepositoryContext<DeviceDb> _context;
    
        public ApplicationRepository(IRepositoryContext<DeviceDb> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ApplicationEntity> GetApplication(Guid applicationId)
        {
            const string applicationSql = @"
                SELECT 
                    Id, 
                    Name, 
                    ApplicationVersionId, 
                    AgentVersionId, 
                    RootFileSystemVersionId,     
                    CreatedUtc
                FROM dbo.Applications 
                WHERE Id = @applicationId";
            var application = await _context.OpenConn()
                .QueryFirstAsync<ApplicationEntity>(applicationSql, new { applicationId });

            const string variableSql = "select * from ApplicationEnvironmentVariables where ApplicationId = @applicationId";

            var variables = await _context.OpenConn()
                .QueryAsync<EnvironmentVariable>(variableSql, new { applicationId });

            application.SetVariables(variables);
            return application;
        }

        public Task<ApplicationVersion> GetApplicationVersion(Guid applicationVersionId)
        {
            return _context.OpenConn()
                .GetAsync<ApplicationVersion>(applicationVersionId);
        }
       
    }
}