using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class DeviceEnvironmentVariable : EnvironmentVariableBase
    {
        [JsonProperty("deviceId")]
        public Guid DeviceId { get; set; }
    }
}