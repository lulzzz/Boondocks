using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using NetFusion.Messaging;

namespace Boondocks.Device.App.Ports
{
    public class DeviceConfigurationPort : IQueryConsumer
    {
        private readonly IRepositoryContext<DeviceDb> _repoContext;
        private readonly IDeviceRepository _deviceRepo;
        private IApplicationRepository _applicationRepo;
        private IVersionRepository _versionRepo;

        public DeviceConfigurationPort(
            IRepositoryContext<DeviceDb> repoContext,
            IDeviceRepository deviceRepo,
            IApplicationRepository applicationRepo,
            IVersionRepository versionRepo)
        {
            _repoContext = repoContext;
            _deviceRepo = deviceRepo;
            _applicationRepo = applicationRepo;
            _versionRepo = versionRepo;
        }

        public async Task<DeviceConfiguration> DetermineConfiguration(CurrentDeviceConfiguration query)
        {
            var device = await _deviceRepo.GetDevice(query.DeviceId);
            var application = await _applicationRepo.GetApplication(device.ApplicationId);

            DeviceConfiguration appConfig = application.BuildConfiguration();
            DeviceConfiguration deviceConfig = device.BuildConfiguration();

            var configuration = appConfig.OverrideWith(deviceConfig); 

            await SetApplicationVersion(configuration);
            await SetAgentVersion(configuration);

            return configuration;
        }

        private async Task SetApplicationVersion(DeviceConfiguration configuration)
        {
            if (configuration.ApplicationVersionId == null)
            {
                return;
            }

            var appVersion = await _versionRepo.GetApplicationVersion(
                configuration.ApplicationVersionId.Value);
                
            var verRef = new VersionReference(appVersion.Id, appVersion.ImageId, appVersion.Name);
            configuration.SetApplicationVersion(verRef);
        }

        private async Task SetAgentVersion(DeviceConfiguration configuration)
        {
            if (configuration.AgentVersionId == null)
            {
                return;
            }

            var agentVersion = await _versionRepo.GetAgentVersion(
                configuration.AgentVersionId.Value);
                
            var verRef = new VersionReference(agentVersion.Id, agentVersion.ImageId, agentVersion.Name);
            configuration.SetAgentVersion(verRef);
        }

    }
}