using System;

namespace FlexyPark.Core.Helpers
{
    public static class TimestampHelpers
    {
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp )
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime UnixTimeStampToDateTime(this string unixTimeStamp )
        {
            long timeStamp = long.Parse(unixTimeStamp);
            return UnixTimeStampToDateTime (timeStamp);
        }

        public static long DateTimeToTimeStamp(this DateTime datetime)
        {
            return (long)(datetime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}

