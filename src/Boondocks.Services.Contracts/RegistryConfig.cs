namespace Boondocks.Services.Contracts
{
    using Newtonsoft.Json;

    public class RegistryConfig
    {
        [JsonProperty("registryHost")]
        public string RegistryHost { get; set; }
    }
}