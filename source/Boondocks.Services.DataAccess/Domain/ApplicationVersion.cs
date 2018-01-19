using System;

namespace Boondocks.Services.DataAccess.Domain
{
    public class ApplicationVersion : EntityBase
    {
        public Guid ApplicationId { get; set; }

        public string Name { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsDeleted { get; set; }
    }
}