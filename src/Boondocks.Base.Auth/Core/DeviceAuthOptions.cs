using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Boondocks.Base.Auth.Core
{
    public class DeviceAuthOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// This will be the URI specified on the device by the code that generates the 
        /// device authentication token.
        /// </summary>
        [Required]
        public string Issuer { get; set; }

        /// <summary>
        /// This will be the URI of the intended audiences specified on the device by
        /// the code that generates the device authentication token.
        /// </summary>
        [Required]
        public string Audience { get; set; }
    }
}
