using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.UI.Touch.Converters
{
    public class StepConverter : MvxValueConverter<double,string>
    {
        protected override string Convert(double value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("{0} {1}", value.ToString(), value >= 0 ? "meters" : "meter");
        }
    }
}

