namespace Boondocks.Services.WebApiClient
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public static class NetworkClientExtensions
    {
        public static async Task<TResult> MakeJsonRequestAsync<TResult>(this ApiClient client, 
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            object queryString = null,
            IDictionary<string, string> headers = null,
            object request = null)
        {
            var response = await client.MakeRequestAsync(cancellationToken, method, path, queryString, headers, request?.ToJsonContent());

            //Deserialize the result
            TResult result = JsonConvert.DeserializeObject<TResult>(response.Body);

            return result;

            
        }
    }
}