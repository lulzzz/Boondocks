namespace Boondocks.Services.Management.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class CreateApplicationEnvironmentVariableRequest : CreateEnvironmentVariableRequest
    {
        /// <summary>
        ///     Get the device
        /// </summary>
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }
    }
}