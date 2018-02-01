namespace Boondocks.Services.Management.Contracts
{
    using Newtonsoft.Json;

    public class CreateDeviceTypeRequest
    {
        [JsonProperty("name")] public string Name { get; set; }
    }
}