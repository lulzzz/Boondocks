namespace Boondocks.Services.WebApiClient
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;

    public static class ObjectExtensions
    {
        public static Func<HttpContent> ToJsonContent(this object source)
        {
            return () =>
            {
                var json = JsonConvert.SerializeObject(source);

                var content = new StringContent(json);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return content;
            };
        }
    }
}