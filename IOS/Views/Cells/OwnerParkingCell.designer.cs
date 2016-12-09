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
    [Register ("OwnerParkingCell")]
    partial class OwnerParkingCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnEdit { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint cstStatusTrailing { get; set; }


        [Outlet]
        UIKit.UILabel lbLocation { get; set; }


        [Outlet]
        UIKit.UILabel lbStatus { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (cstStatusTrailing != null) {
                cstStatusTrailing.Dispose ();
                cstStatusTrailing = null;
            }

            if (lbLocation != null) {
                lbLocation.Dispose ();
                lbLocation = null;
            }

            if (lbStatus != null) {
                lbStatus.Dispose ();
                lbStatus = null;
            }
        }
    }
}