namespace Boondocks.Services.Device.Contracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// The download information for an image.
    /// </summary>
    public class ImageDownloadInfo
    {
        /// <summary>
        /// The Url for the registory to download the image from
        /// </summary>
        [JsonProperty("registry")]
        public string Registry { get; set; }

        /// <summary>
        /// The repository to download the image from
        /// </summary>
        [JsonProperty("repository")]
        public string Repository { get; set; }

        /// <summary>
        /// The id of the image to download.
        /// </summary>
        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        /// <summary>
        /// The authoirzation token to pass to the registry.
        /// </summary>
        [JsonProperty("authToken")]
        public string AuthToken { get; set; }
    }
}