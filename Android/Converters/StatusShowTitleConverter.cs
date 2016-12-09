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
using FlexyPark.Core;

namespace FlexyPark.UI.Droid.Converters
{
    public class StatusShowTitleConverter : MvxValueConverter<ParkingStatus, Boolean>
    {
        protected override bool Convert(ParkingStatus value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)parameter)
            {
                if (value == ParkingStatus.Rented)
                    return true;
            }
            else
            {
                if (value != ParkingStatus.Rented)
                    return true;
            }

            return false;

        }
    }
}