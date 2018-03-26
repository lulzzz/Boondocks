using System;
using System.Collections.Generic;
using System.Linq;

namespace Boondocks.Device.Domain.Entities
{
    public class DeviceEntity
    {
        public IEnumerable<EnvironmentVariable> Variables { get; private set; }

        public DeviceEntity()
        {
            Variables =  Enumerable.Empty<EnvironmentVariable>();
        }

        public string Name { get; private set; }
        public Guid DeviceKey { get; private set; }
        public Guid ApplicationId { get; private set; }
        public Guid? ApplicationVersionId { get; private set; }
        public Guid? AgentVersionId { get; private set; }
        public Guid? RootFileSystemVersionId { get; private set; }
        public Guid ConfigurationVersion { get; private set; }
        public bool IsDisabled { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime CreatedUtc { get; private set; }

        public void SetVariables(IEnumerable<EnvironmentVariable> variables)
        {
            Variables = variables;
        }

        public DeviceConfiguration BuildConfiguration()
        {
            var config = new DeviceConfiguration(
                RootFileSystemVersionId, 
                AgentVersionId, 
                ApplicationVersionId,
                ConfigurationVersion);

            config.SetVariables(Variables.ToArray());
            return config;
        }
    }   
}