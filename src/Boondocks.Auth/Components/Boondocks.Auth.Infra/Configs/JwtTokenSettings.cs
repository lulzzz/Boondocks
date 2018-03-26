using NetFusion.Settings;
using System.ComponentModel.DataAnnotations;

namespace Boondocks.Auth.Infra.Configs
{
    /// <summary>
    /// Configuration settings populated via MS Configuration Extensions.
    /// </summary>
    [ConfigurationSection("boondocks:auth:jwt")]
    public class JwtTokenSettings : AppSettings
    {
        /// <summary>
        /// The issuer of the token specified as a string or URI.
        /// This will be the URI of this authentication microservice issuing the token. 
        /// </summary>
        [Required]
        public string Issuer { get; set; }
        
        /// <summary>
        /// Identifies the recipients that the JWT is intended for.  This value
        /// will be checked by all other Boondocks microservices when receiving 
        /// a JWT token for which they requested by returning a HTTP 401 status
        /// back the client if not already authenticated.
        /// </summary>
        [Required]
        public string Audience { get; set; }
        
        /// <summary>
        /// Determines for how many minutes from the current date
        /// that the token is considered valid.
        /// </summary>
        [Required]
        public int ValidForMinutes { get; set; }
        
        /// <summary>
        /// The path to the certificate file used to sign the JWT token.
        /// </summary>
        [Required]
        public string CertificateFilePath { get; set;}
    }
}