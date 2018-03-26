namespace Boondocks.Base.Auth
{
    /// <summary>
    /// Records the current state of the process when preforming authentication.
    /// </summary>
    public class KeyAuthResult
    {
        // Indicates that authentication has succeeded.
        public bool IsAuthenticated { get; private set; }

        // The signed JWT token.
        public string JwtSignedToken { get; set; }

        // Indicates that the authentication content contains invalid state
        // and that authentication could not be completed.
        public bool IsInvalidCredentialContext { get; private set; }

        public string Reason { get; set; }

        /// <summary>
        /// Created a now result with an authenticated state.
        /// </summary>
        /// <param name="isAuthenticated">Determine the authentication state.</param>
        /// <returns>Authenticated result.</returns>
        public static KeyAuthResult SetAuthenticated(bool isAuthenticated)
        {
            return new KeyAuthResult
            {
                IsAuthenticated = isAuthenticated
            };
        }

        /// <summary>
        /// A default authenticated result.
        /// </summary>
        /// <returns>Authenticated result.</returns>
        public static KeyAuthResult Authenticated() => new KeyAuthResult { IsAuthenticated = true };

       
        /// <summary>
        /// Creates a failed authentication result.
        /// </summary>
        /// <param name="reason">The reason authentication failed.  This value
        /// is returned to the client.</param>
        /// <returns>Invalid authentication result.</returns>
        public static KeyAuthResult Failed(string reason)
        {
            return new KeyAuthResult
            {
                IsAuthenticated = false,
                Reason = reason
            };
        }
    }
}
