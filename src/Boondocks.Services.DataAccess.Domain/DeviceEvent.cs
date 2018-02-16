namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Newtonsoft.Json;

    public class DeviceEvent : EntityBase
    {
        [JsonProperty("deviceId")] public Guid DeviceId { get; set; }

        [JsonProperty("eventType")] public DeviceEventType EventType { get; set; }

        [JsonProperty("message")] public string Message { get; set; }
    }
}