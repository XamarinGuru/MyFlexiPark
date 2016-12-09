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
    [Register ("LeaveParkingView")]
    partial class LeaveParkingView
    {
        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UILabel lbILike { get; set; }


        [Outlet]
        UIKit.UILabel lbYourComment { get; set; }


        [Outlet]
        UIKit.UISwitch swLike { get; set; }


        [Outlet]
        UIKit.UITextView tvComment { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnDone != null) {
                btnDone.Dispose ();
                btnDone = null;
            }

            if (lbILike != null) {
                lbILike.Dispose ();
                lbILike = null;
            }

            if (lbYourComment != null) {
                lbYourComment.Dispose ();
                lbYourComment = null;
            }

            if (swLike != null) {
                swLike.Dispose ();
                swLike = null;
            }

            if (tvComment != null) {
                tvComment.Dispose ();
                tvComment = null;
            }
        }
    }
}