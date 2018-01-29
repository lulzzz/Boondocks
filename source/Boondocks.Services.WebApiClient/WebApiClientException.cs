using System;
using System.Runtime.Serialization;

namespace Boondocks.Services.WebApiClient
{
    public class WebApiClientException : Exception
    {
        public WebApiClientException()
        {
        }

        public WebApiClientException(string message) : base(message)
        {
        }

        public WebApiClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WebApiClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}