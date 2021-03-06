using System;

namespace Boondocks.Device.Domain.Entities
{
    public class AgentVersion : IVersionReference
    {
        public Guid Id { get; private set; }
        public bool IsDisabled { get; private set; }
        public Guid DeviceTypeId { get; private set; }
        public string ImageId { get; private set; }
        public string Name { get; private set; }
        public string Logs { get; private set; }
        public DateTime CreatedUtc { get; private set; }

         /// <summary>
        /// Canonical docker repository name for a given application.
        /// </summary>
        public string RepositoryName => "agent";
    }
}