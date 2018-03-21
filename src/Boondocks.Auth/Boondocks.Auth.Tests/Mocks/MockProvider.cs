using System;
using System.Threading.Tasks;
using Boondocks.Auth.App.Providers;
using Boondocks.Auth.Domain.Entities;

namespace Boondocks.Auth.Tests.Mocks
{
    /// <summary>
    /// Mock provider invoked to determine if an authentication request from a specified named
    /// server has correct credentials.
    /// </summary>
    public class MockProvider : AuthProviderBase
    {
        /// <summary>
        /// Indicates that the request had the property needed credential values required to 
        /// authenticate the caller.
        /// </summary>
        public bool IsValidRequest { get; set; }

        /// <summary>
        ///  Allows the specification of the result that should be returned for a given unit-test.
        /// </summary>
        public Func<AuthContext, AuthResult> HavingResult { get; set; } = r => AuthResult.Authenticated();

        /// <summary>
        /// Reference to the received context for reference by unit-tests.
        /// </summary>
        public AuthContext ReceivedContext {get; set; }

        public override bool IsValidAuthenticationRequest(AuthContext context)
        {
            return IsValidRequest;
        }

        public override Task<AuthResult> OnAuthenticateAsync(AuthContext context)     
        {
            ReceivedContext = context;
            return Task.FromResult(HavingResult(context));
        }
    }
}