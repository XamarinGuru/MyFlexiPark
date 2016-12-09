// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FlexyPark.UI.Touch.Views
{
	[Register ("ParkMeView")]
	partial class ParkMeView
	{
		[Outlet]
		UIKit.UIButton btnParkMeLater { get; set; }

		[Outlet]
		UIKit.UIButton btnParkMeNow { get; set; }

		[Outlet]
		UIKit.UIImageView ivBackground { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ivBackground != null) {
				ivBackground.Dispose ();
				ivBackground = null;
			}

			if (btnParkMeLater != null) {
				btnParkMeLater.Dispose ();
				btnParkMeLater = null;
			}

			if (btnParkMeNow != null) {
				btnParkMeNow.Dispose ();
				btnParkMeNow = null;
			}
		}
	}
}
