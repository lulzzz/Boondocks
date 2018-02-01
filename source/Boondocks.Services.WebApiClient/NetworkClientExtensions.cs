using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Boondocks.Services.WebApiClient
{
    public static class NetworkClientExtensions
    {
        public static async Task<TResult> MakeJsonRequestAsync<TResult>(this NetworkClient client, 
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