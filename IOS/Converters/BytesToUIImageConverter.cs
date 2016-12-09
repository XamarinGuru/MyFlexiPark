using System;
using Cirrious.CrossCore.Converters;
using UIKit;
using FlexyPark.UI.Touch.Extensions;

namespace FlexyPark.UI.Touch.Converters
{
    public class BytesToUIImageConverter : MvxValueConverter<byte[],UIImage>
    {
        protected override UIImage Convert(byte[] value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToImage();
        }
    }
}

