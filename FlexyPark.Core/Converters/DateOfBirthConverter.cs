using System;
using Cirrious.CrossCore.Converters;
using System.Globalization;

namespace FlexyPark.Core.Converters
{
	public class DateOfBirthConverter : MvxValueConverter<string, string>
	{
		protected override string Convert(string value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var date = DateTime.Parse(value);
			var arrDates = date.GetDateTimeFormats();
			return arrDates[5];
		}
	}
}

