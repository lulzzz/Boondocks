using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateDeviceTypeRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}