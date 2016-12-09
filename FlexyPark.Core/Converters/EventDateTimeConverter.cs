using System;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.Core.Converters
{
    public class EventDateTimeConverter : MvxValueConverter<DateTime,string>
    {
        protected override string Convert(DateTime value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            /*var date = value.ToString("O");
            if (date.Contains("+") || date.Contains("-"))
            {
                var temp = date.Split('+');
                var result = temp[0].Replace("T", " ");
                return result.Substring(0,result.Length-3);
            }
            else
                return value.ToString("O");*/
            var date = value.ToString("u");
            return date.Substring(0, date.LastIndexOf(":"));
        }
    }
}

