using System;
using Cirrious.CrossCore.Converters;
using FlexyPark.Core;

namespace FlexyPark.UI.Touch.Converters
{
    public class SearchModeToBoolConverter : MvxValueConverter<SearchMode, bool>
    {
        protected override bool Convert(SearchMode value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter.ToString().Equals("Not") ? (value == SearchMode.Later) : !(value == SearchMode.Later);
        }
    }
}

