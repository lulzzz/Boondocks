using Boondocks.Auth.Domain.Entities;
using Boondocks.Auth.Domain.Services;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Boondocks.Auth.App.Providers
{
    /// <summary>
    /// Base class containing default implementation for an authentication provider.
    /// </summary>
    public abstract class AuthProviderBase : IAuthProvider
    {
        public TokenValidationParameters ValidationParameters { get; private set;}

        public virtual void SetTokenValidationParameters(TokenValidationParameters validationParameters)
        {
            // Allow the derived provider to set additional validation parameters or
            // override the defaults.
            ValidationParameters = validationParameters;
        }


        /// <summary>
        /// Determines if the context contains the required information required to preform authentication.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <returns>True if authentication process can continue.  Otherwise, False.</returns>
        public abstract bool IsValidAuthenticationRequest(AuthContext context);
        
        /// <summary>
        /// Preforms the authentication of the provided context.
        /// </summary>
        /// <param name="context">The context to be authenticated.</param>
        /// <returns>The result of the authentication.</returns>
        public abstract Task<AuthResult> OnAuthenticateAsync(AuthContext context);
    }
}