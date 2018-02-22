namespace Boondocks.Agent
{
    using System.Collections;
    using System.Collections.Generic;
    using Services.Device.Contracts;
    using System.Linq;

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
                .Select(v => $"{v.Name}={v.Value}")
                .ToArray();
        }
    }
}