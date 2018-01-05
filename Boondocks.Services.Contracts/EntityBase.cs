using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class EntityBase
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("createdUtc")]
        public DateTime CreatedUtc { get; set; }
    }
}