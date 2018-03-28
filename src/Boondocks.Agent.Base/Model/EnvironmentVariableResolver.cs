namespace Boondocks.Agent.Base.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Services.Device.Contracts;

    public static class EnvironmentVariableResolver
    {
        public static EnvironmentVariable[] Resolve(IEnumerable<EnvironmentVariable>fromImage, IEnumerable<EnvironmentVariable> fromConfiguration)
        {
            var effective = new Dictionary<string, string>();

            foreach (var variable in fromImage)
            {
                effective[variable.Name] = variable.Value;
            }

            foreach (var variable in fromConfiguration)
            {
                effective[variable.Name] = variable.Value;
            }

            return effective
                .OrderBy(p => p.Key)
                .Select(p => new EnvironmentVariable(p.Key, p.Value))
                .ToArray();
        }
    }
}