using System;
using System.Collections.Generic;
using System.Linq;

namespace Boondocks.Device.Domain.Entities
{
    public class DeviceConfiguration
    {
        public Guid? RootFileSystemVersionId { get; private set; }
        public Guid? AgentVersionId { get; private set; }
        public Guid? ApplicationVersionId { get; private set; }
        public EnvironmentVariable[] Variables = Array.Empty<EnvironmentVariable>();
    
        public DeviceConfiguration(
            Guid? rootFileSystemVersionId, 
            Guid? agentVersionId, 
            Guid? applicationVersionId)
        {
            RootFileSystemVersionId = rootFileSystemVersionId;
            AgentVersionId = agentVersionId;
            ApplicationVersionId = applicationVersionId;
        }

        public VersionReference ApplicationVersion { get; private set; }
        public VersionReference AgentVersion { get; private set; }

        public void SetVariables(EnvironmentVariable[] variables)
        {
            Variables = variables;
        }

        public DeviceConfiguration OverrideWith(DeviceConfiguration configuration)
        {
            var mergedConfig = new DeviceConfiguration(
                configuration.RootFileSystemVersionId ?? RootFileSystemVersionId, 
                configuration.AgentVersionId ?? AgentVersionId, 
                configuration.ApplicationVersionId ?? ApplicationVersionId );

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

        public void SetApplicationVersion(VersionReference version)
        {
            ApplicationVersion = version;
        }

        public void SetAgentVersion(VersionReference version)
        {
            AgentVersion = version;
        }
    }
}