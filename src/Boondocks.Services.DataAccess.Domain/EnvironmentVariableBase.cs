﻿namespace Boondocks.Services.DataAccess.Domain
{
    using Newtonsoft.Json;

    public class EnvironmentVariableBase : EntityBase
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("Value")] public string Value { get; set; }
    }
}