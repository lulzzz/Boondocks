using System;
using System.Collections.Generic;
using System.Linq;

namespace Boondocks.Device.Domain.Entities
{
    public class ApplicationEntity
    {
        public IEnumerable<EnvironmentVariable> Variables {get; private set; }

        public ApplicationEntity()
        {
            Variables =  Enumerable.Empty<EnvironmentVariable>();
        }
        
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid DeviceTypeId { get; set; }
        public Guid? ApplicationVersionId { get; set; }
        public Guid? AgentVersionId { get; set; }
        public Guid? RootFileSystemVersionId { get; set; }
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