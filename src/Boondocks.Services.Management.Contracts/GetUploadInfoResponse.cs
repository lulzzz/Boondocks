namespace Boondocks.Services.Management.Contracts
{
    using Newtonsoft.Json;

    public class GetUploadInfoResponse
    {
        /// <summary>
        ///     If true, the version can be uploaded.
        /// </summary>
        [JsonProperty("canUpload")]
        public bool CanUpload { get; set; }

        /// <summary>
        ///     If CanUpload is false, this will contain the reason why.
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; }

        /// <summary>
        ///     The name of the repository that this application uses.
        /// </summary>
        [JsonProperty("repository")]
        public string Repository { get; set; }

        /// <summary>
        ///     The host for the registry (e.g. "10.0.4.44:5000". No protocol info)
        /// </summary>
        [JsonProperty("registryHost")]
        public string RegistryHost { get; set; }
    }
}