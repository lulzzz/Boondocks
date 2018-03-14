namespace Boondocks.Services.Contracts
{
    using Newtonsoft.Json;

    public class GetHealthResponse
    {
        [JsonProperty("items")]
        public GetHealthResponseItem[] Items { get; set; }
    }
}