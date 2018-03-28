namespace Boondocks.Base.Auth
{
    /// <summary>
    /// Records the current state of the process when preforming device authentication.
    /// </summary>
    public class DeviceAuthResult
    {
        // Indicates that authentication has succeeded.
        public bool IsAuthenticated { get; private set; }

        /// <summary>
        /// Reason for failed authentication.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Created a now result with an authenticated state.
        /// </summary>
        /// <param name="isAuthenticated">Determine the authentication state.</param>
        /// <returns>Authenticated result.</returns>
        public static DeviceAuthResult SetAuthenticated(bool isAuthenticated)
        {
            return new DeviceAuthResult
            {
                IsAuthenticated = isAuthenticated
            };
        }
       
        /// <summary>
        /// Creates a failed authentication result.
        /// </summary>
        /// <param name="reason">The reason authentication failed.  This value
        /// is returned to the client.</param>
        /// <returns>Invalid authentication result.</returns>
        public static DeviceAuthResult Failed(string reason)
        {
            return new DeviceAuthResult
            {
                IsAuthenticated = false,
                Reason = reason
            };
        }
    }
}
