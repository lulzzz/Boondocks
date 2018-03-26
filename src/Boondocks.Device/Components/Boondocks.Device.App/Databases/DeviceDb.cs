using Boondocks.Base.Data;
using NetFusion.Settings;

namespace Boondocks.Device.App.Databases
{
    /// <summary>
    /// Represents and instance of a database and its 
    /// associated connection settings.
    /// </summary>
    [ConfigurationSection("device")]
    public class DeviceDb : DbSettings
    {
        
    }
}