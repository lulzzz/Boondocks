namespace Boondocks.Services.Device.Contracts
{
    using System;
    using Newtonsoft.Json;

    public class GetImageDownloadInfoRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}