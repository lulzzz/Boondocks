﻿namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateDeviceTypeRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}