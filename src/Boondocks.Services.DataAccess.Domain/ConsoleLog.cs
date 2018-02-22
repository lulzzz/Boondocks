namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Newtonsoft.Json;

    public class ConsoleLog
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("createdUtc")]
        public DateTime CreatedUtc { get; set; }
    }
}