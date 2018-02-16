namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Newtonsoft.Json;

    public class SupervisorVersion : NamedEntityBase
    {
        [JsonProperty("deviceArch")]
        public Guid DeviceArchitectureArchitectureId { get; set; }

        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }
    }
}