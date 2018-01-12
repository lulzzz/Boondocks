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
    }
}