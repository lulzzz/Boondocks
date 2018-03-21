using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Services;

namespace Boondocks.Auth.Tests.Mocks
{
    /// <summary>
    /// Mock token service that returns a known token value.
    /// </summary>
    public class MockTokenService : ITokenService
    {
        /// <summary>
        /// The known token value to be returned when requested.
        /// </summary>
        public string ExpectedTokenValue { get; set; }

        public string CreateClaimToken(ResourcePermission[] resourcePermissions)
        {
            return ExpectedTokenValue;
        }
    }
}
