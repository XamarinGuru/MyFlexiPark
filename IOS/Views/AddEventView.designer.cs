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
    [Register ("AddEventView")]
    partial class AddEventView
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint btnAddTopSpace { get; set; }


        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnEndDate { get; set; }


        [Outlet]
        UIKit.UIButton btnEndTime { get; set; }


        [Outlet]
        UIKit.UIButton btnRepeat { get; set; }


        [Outlet]
        UIKit.UIButton btnStartDate { get; set; }


        [Outlet]
        UIKit.UIButton btnStartTime { get; set; }


        [Outlet]
        UIKit.UILabel lbEndDate { get; set; }


        [Outlet]
        UIKit.UILabel lbRepeat { get; set; }


        [Outlet]
        UIKit.UILabel lbStartDate { get; set; }


        [Outlet]
        UIKit.UILabel lbTimes { get; set; }


        [Outlet]
        UIKit.UILabel lbTitle { get; set; }


        [Outlet]
        UIKit.UITextField tfEndDate { get; set; }


        [Outlet]
        UIKit.UITextField tfEndTime { get; set; }


        [Outlet]
        UIKit.UITextField tfRepeat { get; set; }


        [Outlet]
        UIKit.UITextField tfStartDate { get; set; }


        [Outlet]
        UIKit.UITextField tfStartTime { get; set; }


        [Outlet]
        UIKit.UITextField tfTimes { get; set; }


        [Outlet]
        UIKit.UITextField tfTitle { get; set; }


        [Outlet]
        UIKit.UIView vContent { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAdd != null) {
                btnAdd.Dispose ();
                btnAdd = null;
            }

            if (btnDelete != null) {
                btnDelete.Dispose ();
                btnDelete = null;
            }

            if (btnEndDate != null) {
                btnEndDate.Dispose ();
                btnEndDate = null;
            }

            if (btnEndTime != null) {
                btnEndTime.Dispose ();
                btnEndTime = null;
            }

            if (btnRepeat != null) {
                btnRepeat.Dispose ();
                btnRepeat = null;
            }

            if (btnStartDate != null) {
                btnStartDate.Dispose ();
                btnStartDate = null;
            }

            if (btnStartTime != null) {
                btnStartTime.Dispose ();
                btnStartTime = null;
            }

            if (lbEndDate != null) {
                lbEndDate.Dispose ();
                lbEndDate = null;
            }

            if (lbRepeat != null) {
                lbRepeat.Dispose ();
                lbRepeat = null;
            }

            if (lbStartDate != null) {
                lbStartDate.Dispose ();
                lbStartDate = null;
            }

            if (lbTimes != null) {
                lbTimes.Dispose ();
                lbTimes = null;
            }

            if (lbTitle != null) {
                lbTitle.Dispose ();
                lbTitle = null;
            }

            if (tfEndDate != null) {
                tfEndDate.Dispose ();
                tfEndDate = null;
            }

            if (tfEndTime != null) {
                tfEndTime.Dispose ();
                tfEndTime = null;
            }

            if (tfRepeat != null) {
                tfRepeat.Dispose ();
                tfRepeat = null;
            }

            if (tfStartDate != null) {
                tfStartDate.Dispose ();
                tfStartDate = null;
            }

            if (tfStartTime != null) {
                tfStartTime.Dispose ();
                tfStartTime = null;
            }

            if (tfTimes != null) {
                tfTimes.Dispose ();
                tfTimes = null;
            }

            if (tfTitle != null) {
                tfTitle.Dispose ();
                tfTitle = null;
            }

            if (vContent != null) {
                vContent.Dispose ();
                vContent = null;
            }
        }
    }
}