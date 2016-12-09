using System;
using Cirrious.CrossCore.Converters;
using System.Globalization;
using System.Diagnostics;

namespace FlexyPark.Core.Converters
{
    public class DateTimeToStringConverter : MvxValueConverter<DateTime,string>
    {
        //        protected override string Convert(DateTime value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //        {
        //            if (!parameter.Equals("Reservation"))
        //                return ((string)parameter).Equals("Date") ? value.ToString("MM/dd/yyyy") : value.ToString("HH:mm").Replace(":", "h");
        //            else
        //                return string.Format("{0}", value.ToString("dd/MM/yyyy HH:mm").Replace(":", "h"));
        //        }

        protected override string Convert(DateTime value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!parameter.Equals("Reservation"))
                return ((string)parameter).Equals("Date") ? value.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) : value.ToString("HH:mm").Replace(":", "h");
            else
                return string.Format("{0}", value.ToString(string.Format("{0} HH:mm", CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)).Replace(":", "h"));
        }


    }
}