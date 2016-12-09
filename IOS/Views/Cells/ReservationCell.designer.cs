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

namespace FlexyPark.UI.Touch.Views.Cells
{
    [Register ("ReservationCell")]
    partial class ReservationCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UILabel lbAddress { get; set; }


        [Outlet]
        UIKit.UILabel lbEnd { get; set; }


        [Outlet]
        UIKit.UILabel lbEndTime { get; set; }


        [Outlet]
        UIKit.UILabel lbStart { get; set; }


        [Outlet]
        UIKit.UILabel lbStartTime { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbAddress != null) {
                lbAddress.Dispose ();
                lbAddress = null;
            }

            if (lbEnd != null) {
                lbEnd.Dispose ();
                lbEnd = null;
            }

            if (lbEndTime != null) {
                lbEndTime.Dispose ();
                lbEndTime = null;
            }

            if (lbStart != null) {
                lbStart.Dispose ();
                lbStart = null;
            }

            if (lbStartTime != null) {
                lbStartTime.Dispose ();
                lbStartTime = null;
            }
        }
    }
}