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
        
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid DeviceTypeId { get; private set; }
        public Guid? ApplicationVersionId { get; private set; }
        public Guid? AgentVersionId { get; private set; }
        public Guid? RootFileSystemVersionId { get; private set; }
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
                null);

            config.SetVariables(Variables.ToArray());
            return config;
        }
    }
}