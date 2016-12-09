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
    [Register ("ParkingSearchView")]
    partial class ParkingSearchView
    {
        [Outlet]
        UIKit.UIButton btnCheck { get; set; }


        [Outlet]
        UIKit.UIButton btnChooseVehicle { get; set; }


        [Outlet]
        UIKit.UIButton btnStartDate { get; set; }


        [Outlet]
        UIKit.UIButton btnStartHour { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint cstContentHeight { get; set; }


        [Outlet]
        UIKit.UILabel lbEndDate { get; set; }


        [Outlet]
        UIKit.UILabel lbEndHour { get; set; }


        [Outlet]
        UIKit.UILabel lbLocation { get; set; }


        [Outlet]
        UIKit.UILabel lbPlateNumber { get; set; }


        [Outlet]
        UIKit.UILabel lbStartDate { get; set; }


        [Outlet]
        UIKit.UILabel lbStartHour { get; set; }


        [Outlet]
        UIKit.UILabel lbStreet { get; set; }


        [Outlet]
        UIKit.UILabel lbTitle { get; set; }


        [Outlet]
        UIKit.UITableView tablePlaces { get; set; }


        [Outlet]
        UIKit.UITextField tfCity { get; set; }


        [Outlet]
        UIKit.UITextField tfCountry { get; set; }


        [Outlet]
        UIKit.UITextField tfEndDate { get; set; }


        [Outlet]
        UIKit.UITextField tfEndHour { get; set; }


        [Outlet]
        UIKit.UITextField tfNumber { get; set; }


        [Outlet]
        UIKit.UITextField tfPostcode { get; set; }


        [Outlet]
        UIKit.UITextField tfStartDate { get; set; }


        [Outlet]
        UIKit.UITextField tfStartHour { get; set; }


        [Outlet]
        UIKit.UITextField tfStreet { get; set; }


        [Outlet]
        UIKit.UITextField tfStrNumber { get; set; }


        [Outlet]
        UIKit.UIView vBooking { get; set; }


        [Outlet]
        UIKit.UIView vEndDate { get; set; }


        [Outlet]
        UIKit.UIView vStartDate { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnCheck != null) {
                btnCheck.Dispose ();
                btnCheck = null;
            }

            if (btnChooseVehicle != null) {
                btnChooseVehicle.Dispose ();
                btnChooseVehicle = null;
            }

            if (btnStartDate != null) {
                btnStartDate.Dispose ();
                btnStartDate = null;
            }

            if (btnStartHour != null) {
                btnStartHour.Dispose ();
                btnStartHour = null;
            }

            if (cstContentHeight != null) {
                cstContentHeight.Dispose ();
                cstContentHeight = null;
            }

            if (lbPlateNumber != null) {
                lbPlateNumber.Dispose ();
                lbPlateNumber = null;
            }

            if (lbStartDate != null) {
                lbStartDate.Dispose ();
                lbStartDate = null;
            }

            if (lbStartHour != null) {
                lbStartHour.Dispose ();
                lbStartHour = null;
            }

            if (lbStreet != null) {
                lbStreet.Dispose ();
                lbStreet = null;
            }

            if (lbTitle != null) {
                lbTitle.Dispose ();
                lbTitle = null;
            }

            if (tablePlaces != null) {
                tablePlaces.Dispose ();
                tablePlaces = null;
            }

            if (tfStartDate != null) {
                tfStartDate.Dispose ();
                tfStartDate = null;
            }

            if (tfStartHour != null) {
                tfStartHour.Dispose ();
                tfStartHour = null;
            }

            if (tfStreet != null) {
                tfStreet.Dispose ();
                tfStreet = null;
            }

            if (vBooking != null) {
                vBooking.Dispose ();
                vBooking = null;
            }

            if (vStartDate != null) {
                vStartDate.Dispose ();
                vStartDate = null;
            }
        }
    }
}