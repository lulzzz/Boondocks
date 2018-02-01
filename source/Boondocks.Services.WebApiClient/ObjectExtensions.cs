namespace Boondocks.Services.WebApiClient
{
    using System;
    using System.Net.Http;
    using Newtonsoft.Json;

    public static class ObjectExtensions
    {
        public static Func<HttpContent> ToJsonContent(this object source)
        {
            return () =>
            {
                var json = JsonConvert.SerializeObject(source);

                return new StringContent(json);
            };
        }
    }
}