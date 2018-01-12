using System;

namespace Boondocks.Supervisor.Model
{
    public class ApplicationReference
    {
        public string ContainerId { get; set; }

        public string ImageId { get; set; }

        public Guid ApplicationVersionId { get; set; }
    }
}