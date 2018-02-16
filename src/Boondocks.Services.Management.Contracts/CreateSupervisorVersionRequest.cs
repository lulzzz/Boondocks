namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateSupervisorVersionRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("deviceArchitectureId")]
        public Guid DeviceArchitectureId { get; set; }

        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        /// <summary>
        /// The logs from the build process
        /// </summary>
        [JsonProperty("logs")]
        public string Logs { get; set; }

        /// <summary>
        /// If true, upates the application to make this version the new version for all devices.
        /// </summary>
        [JsonProperty("makeCurrent")]
        public bool MakeCurrent { get; set; }

    }
}