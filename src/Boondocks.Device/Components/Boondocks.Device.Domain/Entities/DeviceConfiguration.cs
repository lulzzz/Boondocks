using System;
using System.Linq;

namespace Boondocks.Device.Domain.Entities
{
    public class DeviceConfiguration
    {
        public bool NotFound { get; private set; } = false;

        public Guid? RootFileSystemVersionId { get; private set; }
        public Guid? AgentVersionId { get; private set; }
        public Guid? ApplicationVersionId { get; private set; }
        public Guid? ConfigurationVersion { get; private set; }
        public EnvironmentVariable[] Variables = Array.Empty<EnvironmentVariable>();
    
        public static DeviceConfiguration DeviceNotFound => new DeviceConfiguration { NotFound = true }; 

        private DeviceConfiguration() { }

        public DeviceConfiguration(
            Guid? rootFileSystemVersionId, 
            Guid? agentVersionId, 
            Guid? applicationVersionId,
            Guid? configurationVersion)
        {
            RootFileSystemVersionId = rootFileSystemVersionId;
            AgentVersionId = agentVersionId;
            ApplicationVersionId = applicationVersionId;
            ConfigurationVersion = configurationVersion;
        }

        public RegistryEntry Registry { get; private set; }

        public void SetVariables(EnvironmentVariable[] variables)
        {
            Variables = variables;
        }

        public DeviceConfiguration OverrideWith(DeviceConfiguration configuration)
        {
            var mergedConfig = new DeviceConfiguration(
                configuration.RootFileSystemVersionId ?? RootFileSystemVersionId, 
                configuration.AgentVersionId ?? AgentVersionId, 
                configuration.ApplicationVersionId ?? ApplicationVersionId,
                configuration.ConfigurationVersion ?? ConfigurationVersion );

            var mergedVariables = configuration.Variables.ToList();
            foreach (var variable in Variables)
            {
                if (! mergedVariables.Any(v => v.Name == variable.Name))
                {
                    mergedVariables.Add(variable);
                }
            }

            mergedConfig.SetVariables(mergedVariables.ToArray());
            return mergedConfig;
        }

        public void SetRegistry(RegistryEntry registry)
        {
            Registry = registry;
        }
    }
}