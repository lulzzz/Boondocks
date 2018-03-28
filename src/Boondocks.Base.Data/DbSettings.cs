using NetFusion.Settings;

namespace Boondocks.Base.Data
{
    /// <summary>
    /// Base class representing a SQL database connection for which its configuration
    /// is specified using MS Configuration extensions.
    /// </summary>
    [ConfigurationSection("boondocks:connections")]    
    public abstract class DbSettings : AppSettings
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        public string ConnectionString { get; set;}
    }
}