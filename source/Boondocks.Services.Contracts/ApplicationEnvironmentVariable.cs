namespace Boondocks.Services.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class ApplicationEnvironmentVariable : EnvironmentVariableBase
    {
        [JsonProperty("applicationId")] public Guid ApplicationId { get; set; }
    }
}