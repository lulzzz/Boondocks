using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class EnvironmentVariableBase : EntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}