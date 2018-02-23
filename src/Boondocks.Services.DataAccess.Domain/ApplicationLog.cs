namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Newtonsoft.Json;

    public class ApplicationLog : EntityBase
    {
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("createdLocal")]
        public DateTime CreatedLocal { get; set; }
    }
}