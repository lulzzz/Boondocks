namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class GetAgentUploadInfoRequest
    {
        [JsonProperty("deviceTypeId")]
        public Guid DeviceTypeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageId")]
        public string ImageId { get; set; }
    }
}