using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public abstract class CreateEnvironmentVariableRequest
    {
        /// <summary>
        /// The name of the variable.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}