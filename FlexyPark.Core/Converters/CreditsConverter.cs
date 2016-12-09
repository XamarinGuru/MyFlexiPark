using System;
using Cirrious.CrossCore.Converters;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Localization;

namespace FlexyPark.Core.Converters
{
    public class CreditsConverter : MvxValueConverter<double,string>
    {
        protected override string Convert(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == -1)
                return string.Format("{0}: ...", Mvx.Resolve<ICacheService>().TextSource.GetText("CreditsText"));
            
            return string.Format("{0}: {1:N2}", Mvx.Resolve<ICacheService>().TextSource.GetText("CreditsText"), value);
        }
    }
}

