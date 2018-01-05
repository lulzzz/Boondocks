using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class DeviceType : EntityBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}