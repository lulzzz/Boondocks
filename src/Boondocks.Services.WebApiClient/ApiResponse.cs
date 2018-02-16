namespace Boondocks.Services.WebApiClient
{
    using System.Net;
    using System.Net.Http.Headers;

    public class ApiResponse<TBody> : ApiResponse
    {
        internal ApiResponse(HttpStatusCode statusCode, TBody body, HttpResponseHeaders headers) : base(statusCode, headers)
        {
            Body = body;
        }

        public TBody Body { get; }
    }

    public abstract class ApiResponse
    {
        protected ApiResponse(HttpStatusCode statusCode, HttpResponseHeaders headers)
        {
            StatusCode = statusCode;
            Headers = headers;
        }

        public HttpStatusCode StatusCode { get; }

        public HttpResponseHeaders Headers { get; }
    }
}