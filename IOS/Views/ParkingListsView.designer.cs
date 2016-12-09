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
    [Register ("ParkingListsView")]
    partial class ParkingListsView
    {
        [Outlet]
        UIKit.UIButton btnEndDate { get; set; }


        [Outlet]
        UIKit.UIButton btnEndHour { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint cstVerticalTableToValidUntil { get; set; }


        [Outlet]
        UIKit.UILabel lbEndDate { get; set; }


        [Outlet]
        UIKit.UILabel lbEndHour { get; set; }


        [Outlet]
        UIKit.UILabel lbLonger { get; set; }


        [Outlet]
        UIKit.UILabel lbValidTime { get; set; }


        [Outlet]
        UIKit.UILabel lbValidUntil { get; set; }


        [Outlet]
        UIKit.UISlider sdTime { get; set; }


        [Outlet]
        UIKit.UITableView tableParkingSlot { get; set; }


        [Outlet]
        UIKit.UITextField tfEndDate { get; set; }


        [Outlet]
        UIKit.UITextField tfEndHour { get; set; }


        [Outlet]
        UIKit.UIView vEndDate { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnEndDate != null) {
                btnEndDate.Dispose ();
                btnEndDate = null;
            }

            if (btnEndHour != null) {
                btnEndHour.Dispose ();
                btnEndHour = null;
            }

            if (cstVerticalTableToValidUntil != null) {
                cstVerticalTableToValidUntil.Dispose ();
                cstVerticalTableToValidUntil = null;
            }

            if (lbEndDate != null) {
                lbEndDate.Dispose ();
                lbEndDate = null;
            }

            if (lbEndHour != null) {
                lbEndHour.Dispose ();
                lbEndHour = null;
            }

            if (lbLonger != null) {
                lbLonger.Dispose ();
                lbLonger = null;
            }

            if (lbValidTime != null) {
                lbValidTime.Dispose ();
                lbValidTime = null;
            }

            if (lbValidUntil != null) {
                lbValidUntil.Dispose ();
                lbValidUntil = null;
            }

            if (sdTime != null) {
                sdTime.Dispose ();
                sdTime = null;
            }

            if (tableParkingSlot != null) {
                tableParkingSlot.Dispose ();
                tableParkingSlot = null;
            }

            if (tfEndDate != null) {
                tfEndDate.Dispose ();
                tfEndDate = null;
            }

            if (tfEndHour != null) {
                tfEndHour.Dispose ();
                tfEndHour = null;
            }

            if (vEndDate != null) {
                vEndDate.Dispose ();
                vEndDate = null;
            }
        }
    }
}