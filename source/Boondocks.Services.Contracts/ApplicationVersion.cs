using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class ApplicationVersion : EntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("imageId")]
        public string ImageId { get; set; }
    }
}