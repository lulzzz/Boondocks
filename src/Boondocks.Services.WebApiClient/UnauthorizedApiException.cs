namespace Boondocks.Services.WebApiClient
{
    public class UnauthorizedApiException : ApiException
    {
        internal UnauthorizedApiException(ApiResponse response) : base(response)
        {
        }
    }
}