namespace Boondocks.Services.WebApiClient
{
    using System;
    using System.Net;
    using System.Net.Http.Headers;

    public class ApiException : Exception
    {
        internal ApiException(ApiResponse response)
            : base($"API responded with status code={response.StatusCode}")
        {
            StatusCode = response.StatusCode;
            Headers = response.Headers;
        }

        public HttpStatusCode StatusCode { get; }

        public HttpResponseHeaders Headers { get; }
    }
}