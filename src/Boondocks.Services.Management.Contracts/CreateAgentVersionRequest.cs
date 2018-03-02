namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateAgentVersionRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("deviceTypeId")]
        public Guid DeviceTypeId { get; set; }

        /// <summary>
        /// Image
        /// </summary>
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