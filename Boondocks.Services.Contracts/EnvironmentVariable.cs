using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class EnvironmentVariable : EntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}