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
using Cirrious.CrossCore;
using Cirrious.CrossCore.Converters;
using FlexyPark.Core.Services;

namespace FlexyPark.UI.Droid.Converters
{
    public class AddEditButtonTitleAddEvent : MvxValueConverter<bool, string>
    {
        protected override string Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            string toReturn = string.Empty;
            if (value)
            {
                toReturn = Mvx.Resolve<ICacheService>().TextSource.GetText("EditText");
            }
            else
            {
                toReturn = Mvx.Resolve<ICacheService>().TextSource.GetText("AddText");
            }

            return toReturn;

        }
    }
}