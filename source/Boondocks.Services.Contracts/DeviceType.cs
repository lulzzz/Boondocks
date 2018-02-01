namespace Boondocks.Services.Contracts
{
    using Newtonsoft.Json;

    public class DeviceType : EntityBase
    {
        [JsonProperty("name")] public string Name { get; set; }
    }
}