namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class GetApplicationUploadInfoRequest
    {
        [JsonProperty("applicationId")] public Guid ApplicationId { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("imageId")] public string ImageId { get; set; }
    }
}