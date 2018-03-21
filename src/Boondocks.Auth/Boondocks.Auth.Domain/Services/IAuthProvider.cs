using Boondocks.Auth.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Boondocks.Auth.Domain.Services
{
    /// <summary>
    /// Interface implemented by a component responsible for authenticating credentials
    /// within a specified context.
    /// </summary>
    public interface IAuthProvider
    {
        // If a token is being submitted to identify the client, these are the parameters
        // used to determine if the received token is correctly signed.
        TokenValidationParameters ValidationParameters { get; }

        /// <summary>
        /// Set the valid token validation parameters.
        /// </summary>
        /// <param name="validationParameters">Valid token validation parameters.</param>
        void SetTokenValidationParameters(TokenValidationParameters validationParameters);

        /// <summary>
        /// Determines if the authentication context is valid for processing by the provider.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <returns>True if valid, otherwise False.</returns>
        bool IsValidAuthenticationRequest(AuthContext context);

        /// <summary>
        /// If the context is determine to be valid, this method will be called to determine
        /// if the valid context contains an authenticated identity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The result of the authentication.</returns>
        Task<AuthResult> OnAuthenticateAsync(AuthContext context);
    }
}
