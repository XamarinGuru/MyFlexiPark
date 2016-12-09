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
using Cirrious.MvvmCross.Binding.Combiners;

namespace FlexyPark.UI.Droid.Converters
{
    public class SeekBarConverter : MvxValueConverter<float, int>
    {
        protected override int Convert(float value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            int progress = 1;
            switch (int.Parse((value * 10).ToString()))
            {
                case 0:
                    progress = 0; break;
                case 5:
                    progress = 1; break;
                case 10:
                    progress = 2; break;
                case 15:
                    progress = 3; break;
                case 20:
                    progress = 4; break;
                case 25:
                    progress = 5; break;
                case 30:
                    progress = 6; break;
            }
            return progress;
        }

        protected override float ConvertBack(int value, System.Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            float selectedValue = 0f;
            switch (value)
            {
                case 0:
                    selectedValue = 0.0f; break;
                case 1:
                    selectedValue = 0.5f; break;
                case 2:
                    selectedValue = 1.0f; break;
                case 3:
                    selectedValue = 1.5f; break;
                case 4:
                    selectedValue = 2.0f; break;
                case 5:
                    selectedValue = 2.5f; break;
                case 6:
                    selectedValue = 3.0f; break;
            }
            return selectedValue;
        }
    }
}