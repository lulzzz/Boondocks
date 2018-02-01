using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace Boondocks.Services.WebApiClient
{
    public static class ObjectExtensions
    {
        public static Func<HttpContent> ToJsonContent(this object source)
        {
            return () => {
                string json = JsonConvert.SerializeObject(source);

                return new StringContent(json);
            };
        }
    }
}