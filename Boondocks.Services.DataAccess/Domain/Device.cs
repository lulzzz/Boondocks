using System;

namespace Boondocks.Services.DataAccess.Domain
{
    public class Device : EntityBase
    {
        public string Name { get; set; }

        public Guid DeviceKey { get; set; }

        public Guid ApplicationId { get; set; }

        public Guid? ApplicationVersionId { get; set; }

        public Guid? SupervisorVersionId { get; set; }

        public Guid RootFileSystemId { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDeleted { get; set; }

        public Guid ConfigurationVersion { get; set; }
    }
}