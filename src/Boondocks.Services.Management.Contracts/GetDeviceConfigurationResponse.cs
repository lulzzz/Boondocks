namespace Boondocks.Services.Management.Contracts
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class GetDeviceConfigurationResponse
    {
        /// <summary>
        /// We return the configuration as an opaque string so that the command line utility doesn't have to be
        /// versioned with changes to the device configuration schema.
        /// </summary>
        [JsonProperty("deviceConfiguration")]
        public string DeviceConfiguration { get; set; }
    }
}