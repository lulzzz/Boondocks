namespace Boondocks.Agent.Base
{
    using System.Collections.Generic;
    using System.Linq;
    using Services.Device.Contracts;

    public static class EnvironmentVariableExtensions
    {
        /// <summary>
        /// Formats the environment variables for consumption by a device.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
        public static string[] FormatForDevice(this IEnumerable<EnvironmentVariable> variables)
        {
            return variables
                .Select(FormatForDevice)
                .ToArray();
        }

        public static string FormatForDevice(this EnvironmentVariable variable)
        {
            return $"{variable.Name}={variable.Value}";
        }
    }
}