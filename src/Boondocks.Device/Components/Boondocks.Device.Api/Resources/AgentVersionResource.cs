using System;
using Boondocks.Device.Domain.Entities;
using NetFusion.Rest.Resources.Hal;

namespace Boondocks.Device.Api.Resources
{
    public class AgentVersionResource : HalResource
    {
        public Guid Id { get; set; }

        /// <summary>
        /// The URL for the registry to download the image from
        /// </summary>
        public string Registry { get; set; }

        /// <summary>
        /// The repository to download the image from
        /// </summary>
        public string RepositoryName { get; set; }

        /// <summary>
        /// The id of the image to download.
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// The name (tag of the image)
        /// </summary>
        public string Name { get; set; }

        public static AgentVersionResource FromVersionRef(IVersionReference versionRef)
        {
            return new AgentVersionResource {
                Id = versionRef.Id,
                ImageId = versionRef.ImageId,
                Name = versionRef.Name,
                RepositoryName = versionRef.RepositoryName
            };
        }
    }
}