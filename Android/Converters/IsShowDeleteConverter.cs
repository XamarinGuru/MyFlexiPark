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
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Converters
{
    public class IsShowDeleteConverter: MvxValueConverter<EventItemViewModel, bool>
    {
        protected override bool Convert(EventItemViewModel value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Booking != null ? false : true;
        }
    }
}