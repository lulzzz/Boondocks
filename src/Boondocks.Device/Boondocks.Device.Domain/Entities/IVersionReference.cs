using System;

namespace Boondocks.Device.Domain.Entities
{
    public interface IVersionReference
    {
        /// <summary>
        /// The internal id of the image.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// The docker id of the image.
        /// </summary>
        string ImageId { get; }

        /// <summary>
        /// The name / tag of the image.
        /// </summary>
        string Name { get; }

        string RepositoryName { get; }

    }
}