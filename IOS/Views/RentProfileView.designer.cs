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
    [Register ("RentProfileView")]
    partial class RentProfileView
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.UIButton btnBuyNow { get; set; }


        [Outlet]
        UIKit.UIButton btnCreditPick { get; set; }


        [Outlet]
        UIKit.UIButton btnPick { get; set; }


        [Outlet]
        UIKit.UIButton btnRemove { get; set; }


        [Outlet]
        UIKit.UILabel lbCreditCardCrypto { get; set; }


        [Outlet]
        UIKit.UILabel lbCreditCardName { get; set; }


        [Outlet]
        UIKit.UILabel lbCreditCardNumber { get; set; }


        [Outlet]
        UIKit.UILabel lbCredits { get; set; }


        [Outlet]
        UIKit.UILabel lbValidity { get; set; }


        [Outlet]
        UIKit.UITextField tfCredit { get; set; }


        [Outlet]
        UIKit.UITextField tfCryptogram { get; set; }


        [Outlet]
        UIKit.UITextField tfHolderName { get; set; }


        [Outlet]
        UIKit.UITextField tfNumber { get; set; }


        [Outlet]
        UIKit.UITextField tfValidity { get; set; }


        [Outlet]
        UIKit.UIView vContent { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAdd != null) {
                btnAdd.Dispose ();
                btnAdd = null;
            }

            if (btnBuyNow != null) {
                btnBuyNow.Dispose ();
                btnBuyNow = null;
            }

            if (btnCreditPick != null) {
                btnCreditPick.Dispose ();
                btnCreditPick = null;
            }

            if (btnPick != null) {
                btnPick.Dispose ();
                btnPick = null;
            }

            if (btnRemove != null) {
                btnRemove.Dispose ();
                btnRemove = null;
            }

            if (lbCreditCardCrypto != null) {
                lbCreditCardCrypto.Dispose ();
                lbCreditCardCrypto = null;
            }

            if (lbCreditCardName != null) {
                lbCreditCardName.Dispose ();
                lbCreditCardName = null;
            }

            if (lbCreditCardNumber != null) {
                lbCreditCardNumber.Dispose ();
                lbCreditCardNumber = null;
            }

            if (lbCredits != null) {
                lbCredits.Dispose ();
                lbCredits = null;
            }

            if (lbValidity != null) {
                lbValidity.Dispose ();
                lbValidity = null;
            }

            if (tfCredit != null) {
                tfCredit.Dispose ();
                tfCredit = null;
            }

            if (tfCryptogram != null) {
                tfCryptogram.Dispose ();
                tfCryptogram = null;
            }

            if (tfHolderName != null) {
                tfHolderName.Dispose ();
                tfHolderName = null;
            }

            if (tfNumber != null) {
                tfNumber.Dispose ();
                tfNumber = null;
            }

            if (tfValidity != null) {
                tfValidity.Dispose ();
                tfValidity = null;
            }

            if (vContent != null) {
                vContent.Dispose ();
                vContent = null;
            }
        }
    }
}