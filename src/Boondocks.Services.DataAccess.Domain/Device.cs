namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Interfaces;
    using Newtonsoft.Json;

    public class Device : NamedEntityBase
    {
        [JsonProperty("deviceKey")]
        public Guid DeviceKey { get; set; }

        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }

        [JsonProperty("applicationVersionId")]
        public Guid? ApplicationVersionId { get; set; }

        [JsonProperty("agentVersionId")]
        public Guid? AgentVersionId { get; set; }

        [JsonProperty("rootFileSystemVersionId")]
        public Guid? RootFileSystemVersionId { get; set; }

        [JsonProperty("configurationVersion")]
        public Guid ConfigurationVersion { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }
    }
}