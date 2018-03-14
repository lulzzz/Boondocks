namespace Boondocks.Services.Contracts
{
    using Newtonsoft.Json;

    public class GetHealthResponseItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("passed")]
        public bool Passed { get; set; }
    }
}