using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore.Converters;

namespace FlexyPark.UI.Droid.Converters
{
    public class InverterBoolConverter : MvxValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            return !value;
        }
    }

	public class ReInverterBoolConverter : MvxValueConverter<bool, bool>
	{
		protected override bool Convert(bool value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}