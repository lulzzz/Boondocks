using Boondocks.Auth.Api.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Boondocks.Auth.Tests.Setup
{
    /// <summary>
    /// Extension methods to invoke the Authentication WebApi service.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Authenticate using specified credentials.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use to make request.</param>
        /// <param name="credentialModel">The model containing the authentication credentials.</param>
        /// <returns>Response of the API call.</returns>
        public static Task<HttpResponseMessage> AuthenticateAsync(this HttpClient httpClient, 
            AuthCredentialModel credentialModel)
        {
            return httpClient.AuthenticateAsync("api/boondocks/authentication", credentialModel);
        }

        /// <summary>
        /// Authenticate using specified credentials.
        /// </summary>
        /// <param name="httpClient">>The HTTP client to use to make request.</param>
        /// <param name="uri">The URL to invoke.</param>
        /// <param name="credentialModel">The model containing the authentication credentials.</param>
        /// <returns>Response of the API call.</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> AuthenticateAsync(this HttpClient httpClient, 
            string uri, AuthCredentialModel credentialModel)
        {
            var content = JsonConvert.SerializeObject(credentialModel);
            var buffer = System.Text.Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PostAsync(uri, byteContent);
        }
    }
}