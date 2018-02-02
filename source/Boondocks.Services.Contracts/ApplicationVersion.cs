namespace Boondocks.Services.Contracts
{
    using System;
    using Interfaces;
    using Newtonsoft.Json;

    public class ApplicationVersion : EntityBase, INamedEntity
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("applicationId")] public Guid ApplicationId { get; set; }

        [JsonProperty("isDisabled")] public bool IsDisabled { get; set; }

        [JsonProperty("isDeleted")] public bool IsDeleted { get; set; }

        [JsonProperty("imageId")] public string ImageId { get; set; }

        [JsonProperty("logs")] public string Logs { get; set; }
    }
}