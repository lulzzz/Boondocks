namespace Boondocks.Base
{
    public static class UuidUtil
    {
        private const string ShaPrefix = "sha256:";
        private const int ShortUuidLength = 12;

        /// <summary>
        /// Gets the short version of a uuid, and strips the "sha256:" prefix if present.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetShortUuid(string value)
        {
            if (value == null)
                return null;

            value = GetUuid(value);

            if (value.Length > ShortUuidLength)
            {
                value = value.Substring(0, ShortUuidLength);
            }

            return value;

        }

        /// <summary>
        /// Ditches the "sha256:" prefix.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetUuid(string value)
        {
            return value?.Replace(ShaPrefix, "");
        }
    }
}