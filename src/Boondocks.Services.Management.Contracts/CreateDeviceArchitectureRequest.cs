namespace Boondocks.Services.Management.Contracts
{
    using Newtonsoft.Json;

    public class CreateDeviceArchitectureRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}