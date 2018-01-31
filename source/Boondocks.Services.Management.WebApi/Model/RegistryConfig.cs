using Newtonsoft.Json;

namespace Boondocks.Services.Management.WebApi.Model
{
    public class RegistryConfig
    {
        [JsonProperty("registryHost")]
        public string RegistryHost { get; set; }

        [JsonProperty("isSecure")]
        public bool IsSecure { get; set; }
    }
}