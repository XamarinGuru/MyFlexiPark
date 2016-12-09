using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
    public class RoundedDistanceConverter : MvxValueConverter<string,string>
    {
        protected override string Convert(string value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //double temp = 3000;
            double temp = double.Parse(value);
            if (temp < 1000)
            {
                if (temp % 50 == 0)
                    return string.Format("{0} m", temp);
                else
                {
                    var result = Math.Round((double)temp / 100, MidpointRounding.ToEven) * 100;
                    return string.Format("{0} m", result);
                }
            }
            else if (temp < 10000)
            {
                //temp = temp - 1000;
                double result = Math.Round((double)temp / 1000, 1, MidpointRounding.ToEven);
                if (result - (int)result == 0)
                {
                    return string.Format("{0}.0 km", result);
                }
                return string.Format("{0} km", result);

            }
            return string.Empty;
        }
    }
}

