using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
    public class ParkingMoneyConverter : MvxValueConverter<double,string>
    {
        protected override string Convert(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal a = (decimal)Math.Round(value, 1, MidpointRounding.AwayFromZero);
            return string.Format("{0} €", a);
        }
    }
}
