namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Contracts;
    using Newtonsoft.Json;

    public class ApplicationLog : EntityBase
    {
            
        [JsonProperty("type")]
        public DockerLogEventType Type { get; set; }

        /// <summary>
        /// The id of the device in question.
        /// </summary>
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }

        /// <summary>
        /// The console content.
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// The local time of the log event.
        /// </summary>
        [JsonProperty("createdLocal")]
        public DateTime CreatedLocal { get; set; }
    }
}