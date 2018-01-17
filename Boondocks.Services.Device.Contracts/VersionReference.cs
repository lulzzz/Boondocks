using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Device.Contracts
{
    public class VersionReference
    {
        /// <summary>
        /// The internal id of the image.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The docker id of the image.
        /// </summary>
        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is VersionReference other))
                return false;

            return other.Id == Id && other.ImageId == ImageId;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id}_{ImageId}";
        }
    }
}