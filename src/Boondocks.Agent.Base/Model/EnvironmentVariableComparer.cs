namespace Boondocks.Agent.Base.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Serilog;
    using Services.Device.Contracts;

    public class EnvironmentVariableComparer
    {
        private readonly IList<string> _reservedVariables;

        private readonly ILogger _logger;

        public EnvironmentVariableComparer(IList<string> reservedVariables, ILogger logger = null)
        {
            _reservedVariables = reservedVariables ?? throw new ArgumentNullException(nameof(reservedVariables));

            _logger = logger?.ForContext(GetType());
        }

        public bool AreSame(IEnumerable<string> currentVariables, IEnumerable<EnvironmentVariable> newVariables)
        {
            string[] nonReservedCurrent = currentVariables
                .Where(v => !IsReservedVariable(v))
                .OrderBy(v => v)
                .ToArray();

            _logger?.Information("Current Env Variables: {EnvironmentVariables}", string.Join(", ", nonReservedCurrent));

            string[] formattedNewVariables = newVariables
                .FormatForDevice()
                .OrderBy(v => v)
                .ToArray();

            _logger?.Information("New Env Variables:     {EnvironmentVariables}", string.Join(", ", formattedNewVariables));

            return nonReservedCurrent.SequenceEqual(formattedNewVariables);
        }

        /// <summary>
        /// Determines if the specific variable is reserved.
        /// </summary>
        /// <param name="variable">A variable in the format "NAME=VALUE"</param>
        /// <returns></returns>
        private bool IsReservedVariable(string variable)
        {
            foreach (string reserved in _reservedVariables)
            {
                if (variable.StartsWith(reserved + "="))
                    return true;
            }

            return false;
        }
    }
}