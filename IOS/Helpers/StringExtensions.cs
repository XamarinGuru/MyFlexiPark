using System;
using Foundation;
using CoreGraphics;
using UIKit;

namespace FlexyPark.UI.Touch.Helpers
{
    public static class StringExtensions
    {
        public static nfloat GetHeightForMultilineLabelWithString(this string str, nfloat width, UIFont font)
        {
            var nativeString = new NSString (str);
            CGSize maxLabelSize = new CGSize (width, float.MaxValue);

            CGRect textRect = nativeString.GetBoundingRect (maxLabelSize, NSStringDrawingOptions.UsesLineFragmentOrigin, new UIStringAttributes{ Font = font } , null);

            return textRect.Size.Height;
        }
    }
}

