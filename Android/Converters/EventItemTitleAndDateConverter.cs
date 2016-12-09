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
using FlexyPark.Core.Helpers;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Converters
{
    public class EventItemTitleAndDateConverter:MvxValueConverter<EventItemViewModel, string>
    {
        protected override string Convert(EventItemViewModel value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Unavaiability != null)
            {
                if (parameter == null)
                {
                    System.DateTime mDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    mDateTime = mDateTime.AddSeconds(value.Unavaiability.StartTimestamp).ToLocalTime();
                    return mDateTime.ToString("h:mm tt");
                }
                else
                {
                    return value.Unavaiability.Title;
                }
            }
            else
            {
                if (value.Booking != null)
                {
                    if (parameter == null)
                    {
                        //System.DateTime mDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                        // mDateTime = mDateTime.AddSeconds(value.Booking.StartTimestamp.UnixTimeStampToDateTime().).ToLocalTime();
                        return value.Booking.StartTimestamp.UnixTimeStampToDateTime().ToString("h:mm tt");
                    }
                    else
                    {
                        //return "Booking " + value.Booking.BookingId;
                        //- 17.1 booking: please display the plate number as title - DUY Jul 18
                        return value.Booking.PlateNumber;
                    }
                   
                }
            }
            return string.Empty;
        }
    }
}