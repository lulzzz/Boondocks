namespace Boondocks.Services.Device.Contracts
{
    using System;
    using Newtonsoft.Json;

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

        /// <summary>
        /// The name / tag of the image.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

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