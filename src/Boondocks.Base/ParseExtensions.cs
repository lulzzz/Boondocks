namespace Boondocks.Base
{
    using System;

    /// <summary>
    ///     Methods for parsing values from strings.
    /// </summary>
    public static class ParseExtensions
    {
        private static T? TryParse<T>(string value, TryParseDelegate<T> tryParse, bool throwExceptionOnInvalid)
            where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (tryParse(value, out var parsed))
                return parsed;

            if (throwExceptionOnInvalid)
                throw new ParseException($"Unable to parse value '{value}' as '{typeof(T).Name}'.");

            return null;
        }

        private static T Parse<T>(string value, TryParseDelegate<T> tryParse)
            where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ParseException("No value was provided.");

            if (tryParse(value, out var parsed))
                return parsed;

            throw new ParseException($"Unable to parse value '{value}' as '{typeof(T).Name}'.");
        }

        /// <summary>
        ///     Attempts to parse a Guid from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwExceptionOnInvalid">If a value is provided, but is unparseable, throw a ParseException.</param>
        /// <returns>Returns a Guid if one is found, null otherwise.</returns>
        public static Guid? TryParseGuid(this string value, bool throwExceptionOnInvalid = true)
        {
            return TryParse<Guid>(value, Guid.TryParse, throwExceptionOnInvalid);
        }

        /// <summary>
        ///     Parses a Guid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Guid ParseGuid(this string value)
        {
            return Parse<Guid>(value, Guid.TryParse);
        }

        /// <summary>
        ///     Attempts to parse a bool from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwExceptionOnInvalid">If a value is provided, but is unparseable, throw a ParseException.</param>
        /// <returns>Returns a bool if one is found, null otherwise.</returns>
        public static bool? TryParseBool(this string value, bool throwExceptionOnInvalid = true)
        {
            return TryParse<bool>(value, bool.TryParse, throwExceptionOnInvalid);
        }

        /// <summary>
        ///     Parses a bool.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ParseBool(this string value)
        {
            return Parse<bool>(value, bool.TryParse);
        }

        /// <summary>
        ///     Attempts to parse an int from a string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="throwExceptionOnInvalid">If a value is provided, but is unparseable, throw a ParseException.</param>
        /// <returns>Returns an int if one is found, null otherwise.</returns>
        public static int? TryParseInt(this string value, bool throwExceptionOnInvalid = true)
        {
            return TryParse<int>(value, int.TryParse, throwExceptionOnInvalid);
        }

        /// <summary>
        ///     Parses an int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ParseInt(this string value)
        {
            return Parse<int>(value, int.TryParse);
        }

        private delegate bool TryParseDelegate<T>(string value, out T result) where T : struct;
    }
}