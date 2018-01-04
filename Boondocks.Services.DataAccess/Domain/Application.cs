using System;

namespace Boondocks.Services.DataAccess.Domain
{
    public class Application : EntityBase
    {
        public string Name { get; set; }

        public Guid DeviceTypeId { get; set; }

        public Guid? ApplicationVersionId { get; set; }

        public Guid? SupervisorVersionId { get; set; }
    }
}