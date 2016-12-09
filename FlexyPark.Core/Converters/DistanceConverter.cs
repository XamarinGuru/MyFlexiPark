using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
	public class DistanceConverter : MvxValueConverter<double,string>
    {
		protected override string Convert(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {			
			string info = null;
			if (value > 1000)
				info = string.Format ("{0:0.#} km", value / 1000);
			else if (value > 0)
				info = string.Format ("{0:0.#} m", value);
			else
				info = "Calculating";
			return info;
        }
    }
}

