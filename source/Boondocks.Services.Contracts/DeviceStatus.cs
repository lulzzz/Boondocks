namespace Boondocks.Services.Contracts
{
    using System;
    using Dapper.Contrib.Extensions;
    using Newtonsoft.Json;

    [Table("DeviceStatus")]
    public class DeviceStatus
    {
        [ExplicitKey]
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        [JsonProperty("applicationVersionId")] public Guid? ApplicationVersionId { get; set; }

        [JsonProperty("supervisorVersionId")] public Guid? SupervisorVersionId { get; set; }

        [JsonProperty("rootFileSystemVersionId")]
        public Guid? RootFileSystemVersionId { get; set; }

        [JsonProperty("state")] public DeviceState State { get; set; }

        [JsonProperty("progress")] public int? Progress { get; set; }

        [JsonProperty("lastContactUtc")] public DateTime? LastContactUtc { get; set; }
    }
}