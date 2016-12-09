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
    [Register ("GotoSpotView")]
    partial class GotoSpotView
    {
        [Outlet]
        UIKit.UIButton btnOnSpot { get; set; }


        [Outlet]
        UIKit.UILabel lbPleasGo { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnOnSpot != null) {
                btnOnSpot.Dispose ();
                btnOnSpot = null;
            }

            if (lbPleasGo != null) {
                lbPleasGo.Dispose ();
                lbPleasGo = null;
            }
        }
    }
}