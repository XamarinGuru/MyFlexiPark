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
    [Register ("LostPasswordViewController")]
    partial class LostPasswordViewController
    {
        [Outlet]
        UIKit.UIButton btnOk { get; set; }


        [Outlet]
        UIKit.UILabel lbEmail { get; set; }


        [Outlet]
        UIKit.UITextField tfEmail { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnOk != null) {
                btnOk.Dispose ();
                btnOk = null;
            }

            if (lbEmail != null) {
                lbEmail.Dispose ();
                lbEmail = null;
            }

            if (tfEmail != null) {
                tfEmail.Dispose ();
                tfEmail = null;
            }
        }
    }
}