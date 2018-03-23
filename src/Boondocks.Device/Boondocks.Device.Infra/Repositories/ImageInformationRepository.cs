using System;
using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.Api.Models;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App.Databases;
using Boondocks.Device.App.Settings;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using Dapper.Contrib.Extensions;
using NetFusion.Messaging;

namespace Boondocks.Device.Infra.Repositories
{
    public class VersionRepository : IQueryConsumer, IVersionRepository
    {
        private readonly RegistrySettings _registrySettings;
        private readonly IRepositoryContext<DeviceDb> _repoContext;

        public VersionRepository(
            RegistrySettings registrySettings,
            IRepositoryContext<DeviceDb> repoContext)        
        {
            _registrySettings = registrySettings;
            _repoContext = repoContext;
        }

        public async Task<ImageDownloadModel> Query(AgentImageInfo query)
        {
            var agentVersion = await _repoContext.OpenConn()
                .GetAsync<AgentVersion>(query.Id);

            if (agentVersion == null)
            {
                return new ImageDownloadModel { NoResult = true };
            }

            return new ImageDownloadModel {
                ImageId = agentVersion.ImageId,
                Name = agentVersion.Name,
                Repository = agentVersion.RepositoryName,
                Registry = _registrySettings.Host
            };
        }

        public Task<ApplicationVersion> GetApplicationVersion(Guid applicationVersionId)
        {
            return _repoContext.OpenConn()
                .GetAsync<ApplicationVersion>(applicationVersionId);
        }

        public Task<AgentVersion> GetAgentVersion(Guid agentVersionId)
        {
            return _repoContext.OpenConn()
                .GetAsync<AgentVersion>(agentVersionId);
        }

        public async Task<ImageDownloadModel> Query(ApplicationImageInfo query)
        {
            var appVersion = await _repoContext.OpenConn()
                .GetAsync<ApplicationVersion>(query.Id);

            if (appVersion == null)
            {
                return new ImageDownloadModel { NoResult = true };
            }

            return new ImageDownloadModel {
                ImageId = appVersion.ImageId,
                Name = appVersion.Name,
                Repository = appVersion.RepositoryName,
                Registry = _registrySettings.Host
            };
        }
    }
}