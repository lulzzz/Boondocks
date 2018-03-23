using System.Threading.Tasks;
using Boondocks.Base.Data;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App.Databases;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using NetFusion.Messaging;

namespace Boondocks.Device.App.Ports
{
    /// <summary>
    /// Responds to the CurrentDeviceConfiguration query by determining the version by 
    /// delegating to the device and application entities.  When configurations settings
    /// and environment variables are found at the device level, they take precedence 
    /// over the application corresponding settings and environment variables.     
    /// </summary>
    public class ConfigurationPort : IQueryConsumer
    {
        private readonly IRepositoryContext<DeviceDb> _repoContext;
        private readonly IDeviceRepository _deviceRepo;
        private IApplicationRepository _applicationRepo;

        public ConfigurationPort(
            IRepositoryContext<DeviceDb> repoContext,
            IDeviceRepository deviceRepo,
            IApplicationRepository applicationRepo)
        {
            _repoContext = repoContext;
            _deviceRepo = deviceRepo;
            _applicationRepo = applicationRepo;
        }

        public async Task<DeviceConfiguration> DetermineConfiguration(CurrentDeviceConfiguration query)
        {
            var device = await _deviceRepo.GetDevice(query.DeviceId);
            if (device == null)
            {
                return DeviceConfiguration.DeviceNotFound;
            }

            var application = await _applicationRepo.GetApplication(device.ApplicationId);

            // Get the application level configurations and override with any device level
            // configuration settings.
            DeviceConfiguration appConfig = application.BuildConfiguration();
            DeviceConfiguration deviceConfig = device.BuildConfiguration();

            var configuration = appConfig.OverrideWith(deviceConfig); 

            // Based on the combined settings, set the corresponding application and
            // agent versions.
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

            var appVersion = await _applicationRepo.GetApplicationVersion(
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

            var agentVersion = await _deviceRepo.GetAgentVersion(
                configuration.AgentVersionId.Value);
                
            var verRef = new VersionReference(agentVersion.Id, agentVersion.ImageId, agentVersion.Name);
            configuration.SetAgentVersion(verRef);
        }

    }
}