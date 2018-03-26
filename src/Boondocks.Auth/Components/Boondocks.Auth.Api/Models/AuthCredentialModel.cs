using System.Collections.Generic;

namespace Boondocks.Auth.Api.Models
{
    /// <summary>
    /// Model containing credentials for the caller sent from a service-API
    /// requesting authentication.
    /// </summary>
    public class AuthCredentialModel
    {
        // The API requesting the authentication token (i.e. Device.Api / Management.Api)
        public string Api { get; set;}

        // Set of credentials expected by the provider that will handle
        // authentication associated with the requesting service API.
        public IDictionary<string, string> Credentials { get; set; }

        public AuthCredentialModel()
        {
            Credentials = new Dictionary<string, string>();
        }
    }
}