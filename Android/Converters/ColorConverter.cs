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

namespace FlexyPark.UI.Droid.Converters
{
    public class ColorConverter:MvxValueConverter<bool, Android.Graphics.Color>
    {
        protected override Android.Graphics.Color Convert(bool value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            if (value)
            {
                return Android.Graphics.Color.White;
            }
            else
            {
                return Android.Graphics.Color.Gray;
            }
        }
    }
}