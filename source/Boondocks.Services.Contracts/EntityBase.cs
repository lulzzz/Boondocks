using System;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class EntityBase
    {
        [JsonProperty("id", Order = -100)]
        [ExplicitKey]
        public Guid Id { get; set; }

        [JsonProperty("createdUtc", Order = 100)]
        public DateTime CreatedUtc { get; set; }
    }
}