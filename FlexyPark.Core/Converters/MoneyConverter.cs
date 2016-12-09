using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
    public class MoneyConverter : MvxValueConverter<double,string>
    {
        protected override string Convert(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("{0:0.0} €", value);
        }
    }

    public class MoneyStringConverter : MvxValueConverter<string,string>
    {
        protected override string Convert(string value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("{0} €", value);
        }
    }

}

