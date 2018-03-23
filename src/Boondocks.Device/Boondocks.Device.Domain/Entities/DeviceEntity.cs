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

        public string Name { get; set; }
        public Guid DeviceKey { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid? ApplicationVersionId { get; set; }
        public Guid? AgentVersionId { get; set; }
        public Guid? RootFileSystemVersionId { get; set; }
        public Guid ConfigurationVersion { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedUtc { get; set; }

        public void SetVariables(IEnumerable<EnvironmentVariable> variables)
        {
            Variables = variables;
        }

        public DeviceConfiguration BuildConfiguration()
        {
            var config = new DeviceConfiguration(
                RootFileSystemVersionId, 
                AgentVersionId, 
                ApplicationVersionId);

            config.SetVariables(Variables.ToArray());
            return config;
        }
    }   
}