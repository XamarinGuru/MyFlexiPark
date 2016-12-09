using System;
using Cirrious.CrossCore.Converters;
using Cirrious.CrossCore;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.Converters
{
    public class MeterDoubleConverter : MvxValueConverter<double,string>
    {
        protected override string Convert(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {			
            return string.Format("{0} {1}", System.Convert.ToInt32(value).ToString(), Mvx.Resolve<ICacheService>().SharedTextSource.GetText("MetersText"));
        }
    }
}

