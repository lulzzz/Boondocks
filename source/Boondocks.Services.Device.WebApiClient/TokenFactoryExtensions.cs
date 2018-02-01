using System.Collections.Generic;

namespace Boondocks.Services.Device.WebApiClient
{
    internal static class TokenFactoryExtensions
    {
        public static IDictionary<string, string> CreateRequestHeaders(this TokenFactory tokenFactory)
        {
            return new Dictionary<string, string>()
            {
                { "Authorization", tokenFactory.CreateToken() }
            };
        }
    }
}