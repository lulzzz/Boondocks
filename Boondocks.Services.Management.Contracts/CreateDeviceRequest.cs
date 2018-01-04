using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateDeviceRequest
    {
        [JsonProperty("application-id")]
        public Guid ApplicationId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}