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
    public class OfferTimeToLeaveConverter: MvxValueConverter<string, string>
    {
        protected override string Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("{0} {1}", value, Mvx.Resolve<ICacheService>().TextSource.GetText("OfferMinutesText"));
        }
    }
}