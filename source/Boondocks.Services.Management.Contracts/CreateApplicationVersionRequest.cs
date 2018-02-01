namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateApplicationVersionRequest
    {
        /// <summary>
        ///     The Docker ImageID.
        /// </summary>
        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        /// <summary>
        ///     The name of this version.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     The application id.
        /// </summary>
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }

        /// <summary>
        ///     The logs from the build process
        /// </summary>
        [JsonProperty("logs")]
        public string Logs { get; set; }
    }
}