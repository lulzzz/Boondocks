using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateDeviceResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("deviceKey")]
        public Guid DeviceKey { get; set; }
    }
}