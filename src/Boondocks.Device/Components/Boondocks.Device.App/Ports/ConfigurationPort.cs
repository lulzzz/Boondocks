using Boondocks.Base.Data;
using Boondocks.Device.Api.Queries;
using Boondocks.Device.App.Databases;
using Boondocks.Device.App.Settings;
using Boondocks.Device.Domain;
using Boondocks.Device.Domain.Entities;
using Boondocks.Device.Domain.Repositories;
using NetFusion.Messaging;
using System.Threading.Tasks;

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
        private readonly RegistrySettings _registrySettings;
        private readonly IDeviceRepository _deviceRepo;
        private readonly IApplicationRepository _applicationRepo;

        public ConfigurationPort(
            IRepositoryContext<DeviceDb> repoContext,
            RegistrySettings registrySettings, 
            IDeviceRepository deviceRepo,
            IApplicationRepository applicationRepo)
        {
            _repoContext = repoContext;
            _registrySettings = registrySettings;
            _deviceRepo = deviceRepo;
            _applicationRepo = applicationRepo;
        }

        public async Task<DeviceConfiguration> DetermineConfiguration(GetDeviceConfiguration query)
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
            RegistryEntry registry = await GetRegistryEntry(configuration);
            configuration.SetRegistry(registry);

            return configuration;
        }

        private async Task<RegistryEntry> GetRegistryEntry(DeviceConfiguration configuration)
        {
            RegistryEntry registry = new RegistryEntry { Registry = _registrySettings.Host };

            if (configuration.ApplicationVersionId != null)
            {
                registry.ApplicationVersion =  await _applicationRepo.GetApplicationVersion(
                    configuration.ApplicationVersionId.Value);
            }

            if (configuration.AgentVersionId != null)
            {
                registry.AgentVersion = await _deviceRepo.GetAgentVersion(
                    configuration.AgentVersionId.Value);

            }

            return registry;
        }
    }
}