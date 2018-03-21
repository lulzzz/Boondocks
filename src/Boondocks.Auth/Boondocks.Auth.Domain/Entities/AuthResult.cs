using System;
using System.Security.Claims;

namespace Boondocks.Auth.Domain.Entities
{
    /// <summary>
    /// Records the current state of the process when preforming authentication.
    /// </summary>
    public class AuthResult
    {
        // Indicates that authentication has succeeded.
        public bool IsAuthenticated { get; private set;}

        // Optional result to be returned to client when authenticated.
        public ResourcePermission[] ResourcePermissions { get; private set; } = Array.Empty<ResourcePermission>();

        // The claims to be added to the token.
        public Claim[] Claims { get; private set; } = Array.Empty<Claim>();

        // The signed JWT token.
        public string JwtSignedToken { get; set; }

        // Indicates that the authentication content contains invalid state
        // and that authentication could not be completed.
        public bool IsInvalidCredentialContext { get; private set; }

        // The reasons for an invalid result state.
        public string Reason { get; private set; }

        /// <summary>
        /// Created a now result with an authenticated state.
        /// </summary>
        /// <param name="isAuthenticated">Determine the authentication state.</param>
        /// <returns>Authenticated result.</returns>
        public static AuthResult SetAuthenticated(bool isAuthenticated)
        {
            return new AuthResult {
                IsAuthenticated = isAuthenticated
            };
        }

        /// <summary>
        /// A default authenticated result.
        /// </summary>
        /// <returns>Authenticated result.</returns>
        public static AuthResult Authenticated() => new AuthResult { IsAuthenticated = true };

        /// <summary>
        /// Creates an authenticated result with a set of claims.
        /// </summary>
        /// <param name="accesses">Response to be returned to client containing resource access permissions.</param>
        /// <param name="claims">The claims associated with the authentication.</param>
        /// <returns>Authenticated result.</returns>
        public static AuthResult Authenticated(ResourcePermission[] accesses, params Claim[] claims)
        {
            return new AuthResult {
                IsAuthenticated = true,
                ResourcePermissions = accesses,
                Claims = claims
            };
        }

        /// <summary>
        /// Creates a final and complete authentication result with its associated signed token.
        /// </summary>
        /// <param name="token">Signed token to be returned to client.</param>
        /// <returns>Authenticated result.</returns>
        public AuthResult SetSignedToken(string token)
        {
            if (IsInvalidCredentialContext) {
                throw new InvalidOperationException(
                    "Signed Token can't be set for an Invalid authentication result.");
            }

            var result = Authenticated(ResourcePermissions, Claims);
            result.JwtSignedToken = token;

            return result;
        }

        /// <summary>
        /// Creates a failed authentication result.
        /// </summary>
        /// <param name="reason">The reason authentication failed.  This value
        /// is returned to the client.</param>
        /// <returns>Invalid authentication result.</returns>
        public static AuthResult Failed(string reason)
        {
            return new AuthResult {
                IsInvalidCredentialContext = true,
                Reason = reason
            };
        }

        /// <summary>
        /// Indicates that the status contains state that is considered completed
        /// and can safely be returned to the client.
        /// </summary>
        public bool IsCompletedState =>
            IsInvalidCredentialContext || (IsAuthenticated && !string.IsNullOrEmpty(JwtSignedToken));
    }
}
