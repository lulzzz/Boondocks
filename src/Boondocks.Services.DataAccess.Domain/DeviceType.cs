namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Newtonsoft.Json;

    public class DeviceType : NamedEntityBase
    {
        [JsonProperty("deviceArchitectureId")]
        public Guid DeviceArchitectureId { get; set; }
    }
}