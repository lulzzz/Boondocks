using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateApplicationRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("device-type-id")]
        public Guid DeviceTypeId { get; set; }
    }

    public class CreateApplicationResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}