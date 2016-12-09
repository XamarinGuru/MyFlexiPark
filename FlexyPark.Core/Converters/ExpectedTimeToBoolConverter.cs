using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
    public class ExpectedTimeToBoolConverter : MvxValueConverter<TimeSpan,bool>
    {
        protected override bool Convert(TimeSpan value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.TotalSeconds == 0;
        }
    }
}

