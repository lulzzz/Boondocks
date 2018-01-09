using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Management.Contracts
{
    public class CreateApplicationEnvironmentVariableRequest : CreateEnvironmentVariableRequest
    {
        /// <summary>
        /// Get the device
        /// </summary>
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }
    }
}