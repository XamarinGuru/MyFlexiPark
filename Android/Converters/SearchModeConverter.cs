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
using FlexyPark.Core;

namespace FlexyPark.UI.Droid.Converters
{
    public class SearchModeConverter:MvxValueConverter<SearchMode, bool>
    {
        protected override bool Convert(SearchMode value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value)
            {
                case SearchMode.Now:
                    return false;
                    break;
                default:
                    return true;
                    break;

            }
        }
    }
}