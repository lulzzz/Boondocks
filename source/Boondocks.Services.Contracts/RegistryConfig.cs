namespace Boondocks.Services.Contracts
{
    using Newtonsoft.Json;

    public class RegistryConfig
    {
        [JsonProperty("registryHost")]
        public string RegistryHost { get; set; }

        [JsonProperty("isSecure")]
        public bool IsSecure { get; set; }
    }
}