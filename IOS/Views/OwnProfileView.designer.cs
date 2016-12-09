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
    [Register ("OwnProfileView")]
    partial class OwnProfileView
    {
        [Outlet]
        UIKit.UILabel lbAccountName { get; set; }

        [Outlet]
        UIKit.UILabel lbBankAccount { get; set; }


        [Outlet]
        UIKit.UILabel lbBirthday { get; set; }


        [Outlet]
        UIKit.UILabel lbStreet { get; set; }


        [Outlet]
        UIKit.UILabel lbCity { get; set; }


        [Outlet]
        UIKit.UILabel lbPostalCode { get; set; }


        [Outlet]
        UIKit.UILabel lbCountry { get; set; }


        [Outlet]
        UIKit.UITextField tfBankAccount { get; set; }

        [Outlet]
        UIKit.UITextField tfHolderName { get; set; }



        [Outlet]
        UIKit.UITextField tfBirthday { get; set; }


        [Outlet]
        UIKit.UITextField tfStreet { get; set; }


        [Outlet]
        UIKit.UITextField tfCity { get; set; }


        [Outlet]
        UIKit.UITextField tfPostalCode { get; set; }


        [Outlet]
        UIKit.UITextField tfCountry { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnAdd { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnOK { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnPick { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnReplace { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAdd != null) {
                btnAdd.Dispose ();
                btnAdd = null;
            }

            if (btnOK != null) {
                btnOK.Dispose ();
                btnOK = null;
            }

            if (btnPick != null) {
                btnPick.Dispose ();
                btnPick = null;
            }

            if (btnReplace != null) {
                btnReplace.Dispose ();
                btnReplace = null;
            }

            if (lbAccountName != null) {
                lbAccountName.Dispose ();
                lbAccountName = null;
            }

            if (lbBankAccount != null) {
                lbBankAccount.Dispose ();
                lbBankAccount = null;
            }

            if (lbBirthday != null) {
                lbBirthday.Dispose ();
                lbBirthday = null;
            }

            if (lbCity != null) {
                lbCity.Dispose ();
                lbCity = null;
            }

            if (lbCountry != null) {
                lbCountry.Dispose ();
                lbCountry = null;
            }

            if (lbPostalCode != null) {
                lbPostalCode.Dispose ();
                lbPostalCode = null;
            }

            if (lbStreet != null) {
                lbStreet.Dispose ();
                lbStreet = null;
            }

            if (tfBankAccount != null) {
                tfBankAccount.Dispose ();
                tfBankAccount = null;
            }

            if (tfBirthday != null) {
                tfBirthday.Dispose ();
                tfBirthday = null;
            }

            if (tfCity != null) {
                tfCity.Dispose ();
                tfCity = null;
            }

            if (tfCountry != null) {
                tfCountry.Dispose ();
                tfCountry = null;
            }

            if (tfHolderName != null) {
                tfHolderName.Dispose ();
                tfHolderName = null;
            }

            if (tfPostalCode != null) {
                tfPostalCode.Dispose ();
                tfPostalCode = null;
            }

            if (tfStreet != null) {
                tfStreet.Dispose ();
                tfStreet = null;
            }
        }
    }
}