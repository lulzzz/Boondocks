using System;

namespace Boondocks.Services.DataAccess.Domain
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}