namespace Boondocks.Services.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class Application : EntityBase
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("deviceTypeId")] public Guid DeviceTypeId { get; set; }

        [JsonProperty("applicationVersionId")] public Guid? ApplicationVersionId { get; set; }

        [JsonProperty("supervisorVersionId")] public Guid? SupervisorVersionId { get; set; }

        [JsonProperty("rootFileSystemVersionId")]
        public Guid? RootFileSystemVersionId { get; set; }
    }
}