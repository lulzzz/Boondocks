namespace Boondocks.Services.Contracts
{
    using Interfaces;
    using Newtonsoft.Json;

    public class DeviceType : EntityBase, INamedEntity
    {
        [JsonProperty("name")] public string Name { get; set; }
    }
}