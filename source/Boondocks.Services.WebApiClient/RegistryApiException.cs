namespace Boondocks.Services.WebApiClient
{
    using System;
    using System.Net;
    using System.Net.Http.Headers;

    public class RegistryApiException : Exception
    {
        internal RegistryApiException(ApiResponse response)
            : base($"API responded with status code={response.StatusCode}")
        {
            StatusCode = response.StatusCode;
            Headers = response.Headers;
        }

        public HttpStatusCode StatusCode { get; }

        public HttpResponseHeaders Headers { get; }
    }
}