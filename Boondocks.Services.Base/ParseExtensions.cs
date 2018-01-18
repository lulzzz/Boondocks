using System;

namespace Boondocks.Services.Base
{
    /// <summary>
    /// Methods for parsing values from strings.
    /// </summary>
    public static class ParseExtensions
    {
        /// <summary>
        /// Attempts to parse a Guid from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns a Guid if one is found, null otherwise.</returns>
        public static Guid? TryParseGuid(this string value)
        {
            if (Guid.TryParse(value, out Guid parsed))
                return parsed;

            return null;
        }

        /// <summary>
        /// Attempts to parse a bool from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns a bool if one is found, null otherwise.</returns>
        public static bool? TryParseBool(this string value)
        {
            if (bool.TryParse(value, out bool parsed))
                return parsed;

            return null;
        }

        /// <summary>
        /// Attempts to parse an int from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns an int if one is found, null otherwise.</returns>
        public static int? TryParseInt(this string value)
        {
            if (int.TryParse(value, out int parsed))
                return parsed;

            return null;
        }
    }
}