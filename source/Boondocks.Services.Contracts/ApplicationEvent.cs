namespace Boondocks.Services.Contracts
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