namespace Boondocks.Services.Management.WebApi.Model
{
    using Newtonsoft.Json;

    public class ProvisioningConfig
    {
        [JsonProperty("deviceApiUrl")]
        public string DeviceApiUrl { get; set; }
    }
}