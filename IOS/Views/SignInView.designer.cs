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
    [Register ("SignInView")]
    partial class SignInView
    {
        [Outlet]
        UIKit.UIButton btnCreate { get; set; }


        [Outlet]
        UIKit.UIButton btnForgotPassword { get; set; }


        [Outlet]
        UIKit.UIButton btnSignIn { get; set; }


        [Outlet]
        UIKit.UILabel lbEmail { get; set; }


        [Outlet]
        UIKit.UILabel lbOr { get; set; }


        [Outlet]
        UIKit.UILabel lbPassword { get; set; }


        [Outlet]
        UIKit.UITextField tfEmail { get; set; }


        [Outlet]
        UIKit.UITextField tfPassword { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCreate != null) {
                btnCreate.Dispose ();
                btnCreate = null;
            }

            if (btnForgotPassword != null) {
                btnForgotPassword.Dispose ();
                btnForgotPassword = null;
            }

            if (btnSignIn != null) {
                btnSignIn.Dispose ();
                btnSignIn = null;
            }

            if (lbEmail != null) {
                lbEmail.Dispose ();
                lbEmail = null;
            }

            if (lbOr != null) {
                lbOr.Dispose ();
                lbOr = null;
            }

            if (lbPassword != null) {
                lbPassword.Dispose ();
                lbPassword = null;
            }

            if (tfEmail != null) {
                tfEmail.Dispose ();
                tfEmail = null;
            }

            if (tfPassword != null) {
                tfPassword.Dispose ();
                tfPassword = null;
            }
        }
    }
}