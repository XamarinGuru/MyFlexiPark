using System;
using System.Collections.Generic;
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
    public class StatusToBool:MvxValueConverter<ParkingStatus, bool>
    {
        protected override bool Convert(ParkingStatus value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == ParkingStatus.Reserved)
                return true;
            else
            {
                return false;
            }
        }
    }
}