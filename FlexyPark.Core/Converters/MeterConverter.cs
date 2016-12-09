using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
    public class MeterConverter : MvxValueConverter<double,string>
    {
        protected override string Convert(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {			
            if(value % 50 == 0)
                return string.Format("{0} m", value);
            else
            {
                var result = Math.Round((double)value/100, MidpointRounding.ToEven) * 100;
                return string.Format("{0} m", result);
            }
        }
    }
}

