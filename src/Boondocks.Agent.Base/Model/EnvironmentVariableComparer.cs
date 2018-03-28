namespace Boondocks.Agent.Base.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Serilog;
    using Services.Device.Contracts;

    public class EnvironmentVariableComparer
    {
        private readonly ILogger _logger;

        public EnvironmentVariableComparer(ILogger logger = null)
        {
            _logger = logger?.ForContext(GetType());
        }

        public bool AreSame(IList<EnvironmentVariable> currentVariables, IList<EnvironmentVariable> newVariables)
        {
            string[] current = currentVariables
                .Select(v => v.FormatForDevice())
                .OrderBy(v => v)
                .ToArray();

            _logger?.Information("Current Env Variables: {EnvironmentVariables}", string.Join(", ", current));

            string[] next = newVariables
                .Select(v => v.FormatForDevice())
                .OrderBy(v => v)
                .ToArray();

            _logger?.Information("New Env Variables:     {EnvironmentVariables}", string.Join(", ", next));

            return current.SequenceEqual(next);
        }
    }
}