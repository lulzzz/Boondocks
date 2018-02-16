namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateDeviceResponse
    {
        [JsonProperty("id")] public Guid Id { get; set; }

        [JsonProperty("deviceKey")] public Guid DeviceKey { get; set; }
    }
}