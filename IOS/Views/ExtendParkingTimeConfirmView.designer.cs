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
    [Register ("ExtendParkingTimeConfirmView")]
    partial class ExtendParkingTimeConfirmView
    {
        [Outlet]
        UIKit.UIButton btnBuyCredits { get; set; }


        [Outlet]
        UIKit.UIButton btnPayWithCredits { get; set; }


        [Outlet]
        UIKit.UILabel lbBeforeRemove { get; set; }


        [Outlet]
        UIKit.UILabel lbCarType { get; set; }


        [Outlet]
        UIKit.UILabel lbCost { get; set; }


        [Outlet]
        UIKit.UILabel lbCredits { get; set; }


        [Outlet]
        UIKit.UILabel lbDuration { get; set; }


        [Outlet]
        UIKit.UILabel lbEndOfReservation { get; set; }


        [Outlet]
        UIKit.UILabel lbEndTime { get; set; }


        [Outlet]
        UIKit.UILabel lbExtending { get; set; }


        [Outlet]
        UIKit.UILabel lbExtendTime { get; set; }


        [Outlet]
        UIKit.UILabel lbHours { get; set; }


        [Outlet]
        UIKit.UILabel lbMoney { get; set; }


        [Outlet]
        UIKit.UILabel lbPlateNumber { get; set; }


        [Outlet]
        UIKit.UILabel lbRemainingCredits { get; set; }


        [Outlet]
        UIKit.UILabel lbVehicle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnBuyCredits != null) {
                btnBuyCredits.Dispose ();
                btnBuyCredits = null;
            }

            if (btnPayWithCredits != null) {
                btnPayWithCredits.Dispose ();
                btnPayWithCredits = null;
            }

            if (lbCarType != null) {
                lbCarType.Dispose ();
                lbCarType = null;
            }

            if (lbCost != null) {
                lbCost.Dispose ();
                lbCost = null;
            }

            if (lbCredits != null) {
                lbCredits.Dispose ();
                lbCredits = null;
            }

            if (lbDuration != null) {
                lbDuration.Dispose ();
                lbDuration = null;
            }

            if (lbEndOfReservation != null) {
                lbEndOfReservation.Dispose ();
                lbEndOfReservation = null;
            }

            if (lbEndTime != null) {
                lbEndTime.Dispose ();
                lbEndTime = null;
            }

            if (lbExtending != null) {
                lbExtending.Dispose ();
                lbExtending = null;
            }

            if (lbHours != null) {
                lbHours.Dispose ();
                lbHours = null;
            }

            if (lbMoney != null) {
                lbMoney.Dispose ();
                lbMoney = null;
            }

            if (lbPlateNumber != null) {
                lbPlateNumber.Dispose ();
                lbPlateNumber = null;
            }

            if (lbRemainingCredits != null) {
                lbRemainingCredits.Dispose ();
                lbRemainingCredits = null;
            }

            if (lbVehicle != null) {
                lbVehicle.Dispose ();
                lbVehicle = null;
            }
        }
    }
}