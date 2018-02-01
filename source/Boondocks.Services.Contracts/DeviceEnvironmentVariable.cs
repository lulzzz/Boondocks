namespace Boondocks.Services.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class DeviceEnvironmentVariable : EnvironmentVariableBase
    {
        [JsonProperty("deviceId")] public Guid DeviceId { get; set; }
    }
}