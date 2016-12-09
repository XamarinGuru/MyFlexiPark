using System;
using Cirrious.CrossCore.Converters;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.Converters
{
    public class OfferedTimeConverter : MvxValueConverter<int,string>
    {
        protected override string Convert(int value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("{0} {1}", value, Mvx.Resolve<ICacheService>().TextSource.GetText("OfferMinutesText") );
        }
    }
}

