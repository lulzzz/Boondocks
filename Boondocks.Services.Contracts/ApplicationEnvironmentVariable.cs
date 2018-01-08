using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class ApplicationEnvironmentVariable : EnvironmentVariable
    {
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }
    }
}