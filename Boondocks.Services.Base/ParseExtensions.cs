using System;

namespace Boondocks.Services.Base
{
    public static class ParseExtensions
    {
        public static Guid? ParseGuid(this string value)
        {
            if (Guid.TryParse(value, out Guid parsed))
                return parsed;

            return null;
        }

        public static bool? ParseBool(this string value)
        {
            if (bool.TryParse(value, out bool parsed))
                return parsed;

            return null;
        }

        public static int? ParseInt(this string value)
        {
            if (int.TryParse(value, out int parsed))
                return parsed;

            return null;
        }
    }
}