using System;
using Newtonsoft.Json;

namespace Boondocks.Services.Contracts
{
    public class ApplicationEnvironmentVariable : EnvironmentVariableBase
    {
        [JsonProperty("applicationId")]
        public Guid ApplicationId { get; set; }
    }
}