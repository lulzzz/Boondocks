namespace Boondocks.Agent.Base.Model
{
    using Services.Device.Contracts;

    public static class EnvironnmentVariableParser
    {
        /// <summary>
        /// Parses a value (e.g. "FOO=1") and returns a structured EnvironmentVariable.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static EnvironmentVariable Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new EnvironmentVariable("", "");

            int equalsPos = value.IndexOf('=');

            if (equalsPos < 0)
                return new EnvironmentVariable(value, "");

            string n = value.Substring(0, equalsPos);
            string v = value.Substring(equalsPos + 1);

            return new EnvironmentVariable(n, v);
        }
    }
}