using System;

namespace Boondocks.Device.Domain.Entities
{
    public class ApplicationVersion
    {
        public Guid Id { get; set; }
        public bool IsDisabled { get; set; }
        public Guid ApplicationId { get; set; }
        public string ImageId { get; set; }
        public string Name { get; set; }
        public string Logs { get; set; }
        public DateTime CreatedUtc { get; set; }

        /// <summary>
        /// Canonical docker repository name for a given application.
        /// </summary>
        public string RepositoryName => $"{ApplicationId:D}";
    }
}