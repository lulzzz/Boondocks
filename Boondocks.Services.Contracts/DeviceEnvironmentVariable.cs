using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class DeviceEnvironmentVariable : EnvironmentVariable
    {
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }
    }
}