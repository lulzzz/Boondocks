using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class SupervisorVersion : EntityBase
    {
        [JsonProperty("deviceTypeId")]
        public Guid DeviceTypeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }       
    }
}