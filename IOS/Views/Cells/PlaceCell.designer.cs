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

namespace FlexyPark.UI.Touch.Views.Cells
{
    [Register ("PlaceCell")]
    partial class PlaceCell
    {
        [Outlet]
        UIKit.UILabel lbCountry { get; set; }


        [Outlet]
        UIKit.UILabel lbStreet { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbCountry != null) {
                lbCountry.Dispose ();
                lbCountry = null;
            }

            if (lbStreet != null) {
                lbStreet.Dispose ();
                lbStreet = null;
            }
        }
    }
}