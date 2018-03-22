using NetFusion.Settings;

namespace Boondocks.Device.App.Settings
{
    [ConfigurationSection("boondocks:registry")]
    public class RegistrySettings : AppSettings
    {
        public string Host { get; set; }
    }
}