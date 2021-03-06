﻿namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Newtonsoft.Json;

    public class AgentVersion : NamedEntityBase
    {
        [JsonProperty("deviceTypeId")]
        public Guid DeviceTypeId { get; set; }

        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("logs")]
        public string Logs { get; set; }
    }
}