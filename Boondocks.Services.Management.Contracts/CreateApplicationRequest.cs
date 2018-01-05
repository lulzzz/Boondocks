using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateApplicationRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("deviceTypeId")]
        public Guid DeviceTypeId { get; set; }
    }
}