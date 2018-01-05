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
    }
}