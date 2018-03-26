namespace Boondocks.Device.Api.Models
{
    /// <summary>
    /// The download information for an image that can be used 
    /// for retrieving the image.
    /// </summary>
    public class ImageDownloadModel
    {
        public bool NoResult { get; set; }

        /// <summary>
        /// The URL for the registry to download the image from
        /// </summary>
        public string Registry { get; set; }

        /// <summary>
        /// The repository to download the image from
        /// </summary>
        public string Repository { get; set; }

        /// <summary>
        /// The id of the image to download.
        /// </summary>
        public string ImageId { get; set; }

        /// <summary>
        /// The name (tag of the image)
        /// </summary>
        public string Name { get; set; }
    }
}