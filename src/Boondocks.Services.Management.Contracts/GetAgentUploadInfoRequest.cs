namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class GetAgentUploadInfoRequest
    {
        [JsonProperty("deviceArchitectureId")]
        public Guid DeviceArchitectureId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageId")]
        public string ImageId { get; set; }
    }
}