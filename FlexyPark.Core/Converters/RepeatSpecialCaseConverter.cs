using System;
using Cirrious.CrossCore.Converters;
using System.Collections.Generic;

namespace FlexyPark.Core.Converters
{
    public class RepeatSpecialCaseConverter : MvxValueConverter<string,string>
    {
        protected override string Convert(string value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var list = new List<string>(AppConstants.Repeats);
            var index = list.IndexOf(value);
            switch (index)
            {
                case 0:
                    return "Never";
                case 1:
                    return "Every Day";
                case 2:
                    return "Every Week";
                case 3:
                    return "Every 2 Week";
                case 4:
                    return "Every Month";
                case 5:
                    return "Every Year";
                default:
                    return string.Empty;
            }
        }
    }
}

