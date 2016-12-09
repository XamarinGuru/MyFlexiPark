using System;
using Foundation;
using UIKit;

namespace FlexyPark.UI.Touch.Helpers
{
	public static class FontHelper
	{
		private static readonly float widthRatio = (float)UIScreen.MainScreen.Bounds.Width / 414.0f;
		public static UIFont AdjustFontSize(UIFont font)
		{		
            return font.WithSize(font.PointSize * widthRatio);
		}

		public static nfloat GetAdjustFontSize(UIFont font)
		{
			return font.PointSize * widthRatio;
		}
	}
}

