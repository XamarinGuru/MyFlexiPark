using System;
using Cirrious.CrossCore.Converters;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.Converters
{
    public class DurationConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("{0} {1}", value, value < 2 ? Mvx.Resolve<ICacheService>().SharedTextSource.GetText("HourText") : Mvx.Resolve<ICacheService>().SharedTextSource.GetText("HoursText"));
        }
    }
}

