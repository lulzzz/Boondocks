using System.ComponentModel.DataAnnotations;
using NetFusion.Settings;

namespace Boondocks.Auth.Infra.Providers
{
    /// <summary>
    /// Configuration settings used by the authentication provider
    /// when authenticating using Active Directory.
    /// </summary>
    [ConfigurationSection("boondocks:auth:ldap")]
    public class LdapSettings : AppSettings
    {
        // The domain to for the user name if not specified.
        public string Domain { get; set; }
        
        // The connection to the LDAP server.
        [Required]
        public string Connection { get; set; }

        // The port for the LDAP server connection.
        [Required]
        public int Port { get; set;} = 389;
    }
}