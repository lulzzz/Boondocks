namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Newtonsoft.Json;

    public class ApplicationEvent : EntityBase
    {
        [JsonProperty("applicationId")] public Guid ApplicationId { get; set; }

        [JsonProperty("eventType")] public ApplicationEventType EventType { get; set; }

        [JsonProperty("message")] public string Message { get; set; }
    }
}