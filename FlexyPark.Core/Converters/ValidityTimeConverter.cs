using System;
using Cirrious.CrossCore.Converters;
using System.Globalization;

namespace FlexyPark.Core.Converters
{
    public class ValidityTimeConverter : MvxValueConverter<DateTime,string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString("MM/yyyy");
        }
    }
}

