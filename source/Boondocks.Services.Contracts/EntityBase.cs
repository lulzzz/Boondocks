﻿namespace Boondocks.Services.Contracts
{
    using System;
    using Dapper.Contrib.Extensions;
    using Interfaces;
    using Newtonsoft.Json;

    public class EntityBase : IEntity
    {
        [JsonProperty("id", Order = -100)]
        [ExplicitKey]
        public Guid Id { get; set; }

        [JsonProperty("createdUtc", Order = 100)]
        public DateTime CreatedUtc { get; set; }
    }
}