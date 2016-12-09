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
    public class StatusToImageResourConverter:MvxValueConverter<ParkingStatus, int>
    {
        protected override int Convert(ParkingStatus value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {

            if (value == ParkingStatus.Reserved)
                return Resource.Drawable.white_icon_back;
            else
            {
                return Resource.Drawable.white_icon_menu;
            }
        }
    }
}