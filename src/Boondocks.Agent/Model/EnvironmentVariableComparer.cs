namespace Boondocks.Agent.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Services.Device.Contracts;

    public class EnvironmentVariableComparer
    {
        private readonly IList<string> _reservedVariables;

        public EnvironmentVariableComparer(IList<string> reservedVariables)
        {
            _reservedVariables = reservedVariables ?? throw new ArgumentNullException(nameof(reservedVariables));
        }

        public bool AreSame(IEnumerable<string> currentVariables, IEnumerable<EnvironmentVariable> newVariables)
        {
            string[] nonReservedCurrent = currentVariables
                .Where(v => !IsReservedVariable(v))
                .OrderBy(v => v)
                .ToArray();

            string[] formattedNewVariables = newVariables
                .FormatForDevice()
                .OrderBy(v => v)
                .ToArray();

            return nonReservedCurrent.SequenceEqual(formattedNewVariables);
        }

        /// <summary>
        /// 
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