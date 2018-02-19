namespace Boondocks.Services.DataAccess.Domain
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

        [JsonProperty("applicationVersion")]
        public string ApplicationVersion { get; set; }

        [JsonProperty("agentVersion")]
        public string AgentVersion { get; set; }

        [JsonProperty("rootFileSystemVersion")]
        public string RootFileSystemVersion { get; set; }

        [JsonProperty("state")]
        public DeviceState State { get; set; }

        [JsonProperty("progress")]
        public int? Progress { get; set; }

        [JsonProperty("lastContactUtc")]
        public DateTime? LastContactUtc { get; set; }
    }
}