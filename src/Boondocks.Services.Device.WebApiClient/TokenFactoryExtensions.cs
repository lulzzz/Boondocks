namespace Boondocks.Services.Device.WebApiClient
{
    using System.Collections.Generic;

    internal static class TokenFactoryExtensions
    {
        public static IDictionary<string, string> CreateRequestHeaders(this TokenFactory tokenFactory)
        {
            return new Dictionary<string, string>
            {
                {"Authorization", tokenFactory.CreateToken()}
            };
        }
    }
}