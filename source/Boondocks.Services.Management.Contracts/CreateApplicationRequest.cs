namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateApplicationRequest
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("deviceTypeId")] public Guid DeviceTypeId { get; set; }
    }
}