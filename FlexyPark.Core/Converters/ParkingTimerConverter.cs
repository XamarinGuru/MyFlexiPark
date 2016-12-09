using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
    public class ParkingTimerConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var ts = TimeSpan.FromSeconds(value);
            return string.Format("{0}", ts.ToString("c") );
        }
    }
}

