using System;
using System.Globalization;

namespace FlexyPark.UI.Touch.Extensions
{
    public static class DoubleExtensions
    {
        public static string ParseToCultureInfo(this double value, CultureInfo cultureInfo)
        {
            //return value.ToString("#,##0.0", cultureInfo);
            return ((decimal)value).ToString(cultureInfo);
        }

        public static double ToRad(this double value)
        {
            return value / 180.0f * Math.PI;
        }

        public static double ToDeg(this double value)
        {
            return value / Math.PI * 180.0f;
        }


    }
}

