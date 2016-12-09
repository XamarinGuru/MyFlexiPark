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
    public class EventRedGrayConverter : MvxValueConverter<EventItemViewModel, bool>
    {
        protected override bool Convert(EventItemViewModel value, Type targetType, object parameter, CultureInfo culture)
        {

            if (parameter == null)
            {
                // booking - red

                if (value.Booking != null)
                {
                    return true;
                }

            }
            else
            {
                // Unavailability - gray

                if (value.Unavaiability != null)
                {
                    return true;
                }

            }

            return false;
        }

    }
}