using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.UI.Droid.Converters
{
    public class DateTimeToHourConverter:MvxValueConverter<long, string>
    {
        protected override string Convert(long value, Type targetType, object parameter, CultureInfo culture)
        {
            System.DateTime mDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            mDateTime = mDateTime.AddSeconds(value).ToLocalTime();



            //if (mDateTime.Hour > 12)
            //{
            //    return mDateTime.Hour - 12 + "h" + mDateTime.Minute.ToString("00") + " PM";
            //}
            //else
            //{
            //    return mDateTime.Hour + "h" + mDateTime.Minute.ToString("00") + " AM";
            //}
            return mDateTime.Hour + "h" + mDateTime.Minute.ToString("00");
        }
    }
}