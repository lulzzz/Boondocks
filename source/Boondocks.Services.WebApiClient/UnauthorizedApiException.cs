namespace Boondocks.Services.WebApiClient
{
    public class UnauthorizedApiException : RegistryApiException
    {
        internal UnauthorizedApiException(ApiResponse response) : base(response)
        {
        }
    }
}