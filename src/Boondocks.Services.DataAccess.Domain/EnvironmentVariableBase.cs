namespace Boondocks.Services.DataAccess.Domain
{
    using Newtonsoft.Json;

    public abstract class EnvironmentVariableBase : NamedEntityBase
    {
        [JsonProperty("Value")]
        public string Value { get; set; }
    }
}