﻿namespace Boondocks.Services.DataAccess.Domain
{
    using System;
    using Interfaces;
    using Newtonsoft.Json;

    public class Application : EntityBase, INamedEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("deviceTypeId")]
        public Guid DeviceTypeId { get; set; }

        [JsonProperty("applicationVersionId")]
        public Guid? ApplicationVersionId { get; set; }

        [JsonProperty("agentVersionId")]
        public Guid? AgentVersionId { get; set; }

        [JsonProperty("rootFileSystemVersionId")]
        public Guid? RootFileSystemVersionId { get; set; }
    }
}