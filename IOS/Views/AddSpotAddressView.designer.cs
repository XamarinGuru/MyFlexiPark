// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace FlexyPark.UI.Touch.Views
{
    [Register ("AddSpotAddressView")]
    partial class AddSpotAddressView
    {
        [Outlet]
        UIKit.UILabel lbLocationLand { get; set; }


        [Outlet]
        UIKit.UITextField tfLocation { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbLocationLand != null) {
                lbLocationLand.Dispose ();
                lbLocationLand = null;
            }

            if (tfLocation != null) {
                tfLocation.Dispose ();
                tfLocation = null;
            }
        }
    }
}