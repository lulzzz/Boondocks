using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class ApplicationUploadInfo
    {
        /// <summary>
        /// The name of the repository that this application uses.
        /// </summary>
        [JsonProperty("repository")]
        public string Repository { get; set; }

        /// <summary>
        /// The host for the registry (e.g. "10.0.4.44:5000". No protocol info)
        /// </summary>
        [JsonProperty("registryHost")]
        public string RegistryHost { get; set; }
    }
}