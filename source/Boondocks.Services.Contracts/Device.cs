namespace Boondocks.Services.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class Device : EntityBase
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("deviceKey")] public Guid DeviceKey { get; set; }

        [JsonProperty("applicationId")] public Guid ApplicationId { get; set; }

        [JsonProperty("applicationVersionId")] public Guid? ApplicationVersionId { get; set; }

        [JsonProperty("supervisorVersionId")] public Guid? SupervisorVersionId { get; set; }

        [JsonProperty("rootFileSystemVersionId")]
        public Guid? RootFileSystemVersionId { get; set; }

        [JsonProperty("configurationVersion")] public Guid ConfigurationVersion { get; set; }

        [JsonProperty("isDisabled")] public bool IsDisabled { get; set; }

        [JsonProperty("isDeleted")] public bool IsDeleted { get; set; }
    }
}