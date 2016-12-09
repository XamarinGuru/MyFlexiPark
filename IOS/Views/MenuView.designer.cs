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
    [Register ("MenuView")]
    partial class MenuView
    {
        [Outlet]
        UIKit.UIButton btnCredits { get; set; }


        [Outlet]
        UIKit.UIButton btnMyOwnParking { get; set; }


        [Outlet]
        UIKit.UIButton btnMyReservations { get; set; }


        [Outlet]
        UIKit.UIButton btnParkMe { get; set; }


        [Outlet]
        UIKit.UIButton btnParkmeLater { get; set; }


        [Outlet]
        UIKit.UIButton btnParkmeNow { get; set; }


        [Outlet]
        UIKit.UIButton btnSettings { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint cstHeightViewContent { get; set; }


        [Outlet]
        UIKit.UIImageView ivBackground { get; set; }


        [Outlet]
        UIKit.UILabel lbCredits { get; set; }


        [Outlet]
        UIKit.UILabel lbMyOwnParking { get; set; }


        [Outlet]
        UIKit.UILabel lbMyReservations { get; set; }


        [Outlet]
        UIKit.UILabel lbParkMe { get; set; }


        [Outlet]
        UIKit.UIView vCredits { get; set; }


        [Outlet]
        UIKit.UIView vParkMePopup { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCredits != null) {
                btnCredits.Dispose ();
                btnCredits = null;
            }

            if (btnMyOwnParking != null) {
                btnMyOwnParking.Dispose ();
                btnMyOwnParking = null;
            }

            if (btnMyReservations != null) {
                btnMyReservations.Dispose ();
                btnMyReservations = null;
            }

            if (btnParkMe != null) {
                btnParkMe.Dispose ();
                btnParkMe = null;
            }

            if (btnParkmeLater != null) {
                btnParkmeLater.Dispose ();
                btnParkmeLater = null;
            }

            if (btnParkmeNow != null) {
                btnParkmeNow.Dispose ();
                btnParkmeNow = null;
            }

            if (btnSettings != null) {
                btnSettings.Dispose ();
                btnSettings = null;
            }

            if (cstHeightViewContent != null) {
                cstHeightViewContent.Dispose ();
                cstHeightViewContent = null;
            }

            if (ivBackground != null) {
                ivBackground.Dispose ();
                ivBackground = null;
            }

            if (lbCredits != null) {
                lbCredits.Dispose ();
                lbCredits = null;
            }

            if (lbMyOwnParking != null) {
                lbMyOwnParking.Dispose ();
                lbMyOwnParking = null;
            }

            if (lbMyReservations != null) {
                lbMyReservations.Dispose ();
                lbMyReservations = null;
            }

            if (lbParkMe != null) {
                lbParkMe.Dispose ();
                lbParkMe = null;
            }

            if (vCredits != null) {
                vCredits.Dispose ();
                vCredits = null;
            }

            if (vParkMePopup != null) {
                vParkMePopup.Dispose ();
                vParkMePopup = null;
            }
        }
    }
}