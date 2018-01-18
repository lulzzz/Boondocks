﻿using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateApplicationVersionRequest
    {
        /// <summary>
        /// The Docker ImageID.
        /// </summary>
        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        /// <summary>
        /// The name of this version.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The application id.
        /// </summary>
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }
    }
}