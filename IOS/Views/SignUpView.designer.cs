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
    [Register ("SignUpView")]
    partial class SignUpView
    {
        [Outlet]
        UIKit.UIButton btnAcceptTerms { get; set; }


        [Outlet]
        UIKit.UIButton btnLogout { get; set; }


        [Outlet]
        UIKit.UIButton btnOk { get; set; }


        [Outlet]
        UIKit.UIButton btnReadTerms { get; set; }


        [Outlet]
        UIKit.UISwitch swAccept { get; set; }


        [Outlet]
        UIKit.UISwitch swAcceptTerms { get; set; }


        [Outlet]
        UIKit.UITextField tfEmail { get; set; }


        [Outlet]
        UIKit.UITextField tfFirstName { get; set; }


        [Outlet]
        UIKit.UITextField tfLastName { get; set; }


        [Outlet]
        UIKit.UITextField tfMobileNumber { get; set; }


        [Outlet]
        UIKit.UITextField tfPassword { get; set; }


        [Outlet]
        UIKit.UITextField tfPromoCode { get; set; }


        [Outlet]
        UIKit.UITextField tfReference { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAcceptTerms != null) {
                btnAcceptTerms.Dispose ();
                btnAcceptTerms = null;
            }

            if (btnLogout != null) {
                btnLogout.Dispose ();
                btnLogout = null;
            }

            if (btnOk != null) {
                btnOk.Dispose ();
                btnOk = null;
            }

            if (btnReadTerms != null) {
                btnReadTerms.Dispose ();
                btnReadTerms = null;
            }

            if (swAccept != null) {
                swAccept.Dispose ();
                swAccept = null;
            }

            if (swAcceptTerms != null) {
                swAcceptTerms.Dispose ();
                swAcceptTerms = null;
            }

            if (tfEmail != null) {
                tfEmail.Dispose ();
                tfEmail = null;
            }

            if (tfFirstName != null) {
                tfFirstName.Dispose ();
                tfFirstName = null;
            }

            if (tfLastName != null) {
                tfLastName.Dispose ();
                tfLastName = null;
            }

            if (tfMobileNumber != null) {
                tfMobileNumber.Dispose ();
                tfMobileNumber = null;
            }

            if (tfPassword != null) {
                tfPassword.Dispose ();
                tfPassword = null;
            }

            if (tfPromoCode != null) {
                tfPromoCode.Dispose ();
                tfPromoCode = null;
            }
        }
    }
}