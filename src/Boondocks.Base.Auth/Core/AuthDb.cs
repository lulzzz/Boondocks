using Boondocks.Base.Data;
using NetFusion.Settings;

namespace Boondocks.Base.Auth.Core
{
    /// <summary>
    /// Database settings representing the database to which the base
    /// authorization component searches for device related authorization
    /// information.
    /// </summary>
    [ConfigurationSection("auth")]
    public class AuthDb : DbSettings
    {

    }
}