using System;

namespace Boondocks.Device.Domain.Entities
{
    public class VersionReference
    {
        /// <summary>
        /// The internal id of the image.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The docker id of the image.
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// The name / tag of the image.
        /// </summary>
        public string Name { get; set; }

        public VersionReference(Guid id, string imageId, string name)
        {
            Id = id;
            ImageId = imageId;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VersionReference other))
                return false;

            return other.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}_{ImageId}:{Name}";
        }
    }
}