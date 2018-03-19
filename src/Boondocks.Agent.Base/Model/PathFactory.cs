namespace Boondocks.Agent.Base.Model
{
    /// <summary>
    /// Responsible for generating the paths we use throughout the agent.
    /// </summary>
    public class PathFactory
    {
        /// <summary>
        /// The path of the device configuration file.
        /// </summary>
        public string DeviceConfigFile => "/mnt/root/mnt/boot/boondocks.device.config";
    }
}