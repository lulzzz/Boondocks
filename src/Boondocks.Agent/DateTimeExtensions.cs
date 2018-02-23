namespace Boondocks.Agent
{
    using System;

    public static class DateTimeExtensions
    {
        private static readonly DateTime EpochTime = new DateTime(1970, 1, 1);

        public static int ToUnixEpochTime(this DateTime value)
        {
            return (Int32)value.Subtract(EpochTime).TotalSeconds;
        }

        public static string ToUnixDate(this DateTime value)
        {
            // 2013-01-02T13:23:37

            //return $"{value.Year}-{value.Month:00}-{value.Day:00}T{value.Hour:00}:{value.Minute:00}:{value.Second:00}";

            //2014-09-16T06:17:46.000000000Z

            //return $"{value.Year}-{value.Month:00}-{value.Day:00}T{value.Hour:00}:{value.Minute:00}:{value.Second:00}.{value.Millisecond:000}000000Z";

            return $"{value.Year}-{value.Month:00}-{value.Day:00}";
        }
    }
}