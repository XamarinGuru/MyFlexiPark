using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.UI.Touch.Converters
{
    public class BooleanToHiddenConverter : MvxValueConverter<bool,bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !value;
        }

        protected override bool ConvertBack(bool value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !value;
        }
    }
}

