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
    [Register ("AddVehicleView")]
    partial class AddVehicleView
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.UILabel lbPlateNumber { get; set; }


        [Outlet]
        UIKit.UITableView tableCarType { get; set; }


        [Outlet]
        UIKit.UITextField tfPlateNumber { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbPlateNumber != null) {
                lbPlateNumber.Dispose ();
                lbPlateNumber = null;
            }

            if (tableCarType != null) {
                tableCarType.Dispose ();
                tableCarType = null;
            }

            if (tfPlateNumber != null) {
                tfPlateNumber.Dispose ();
                tfPlateNumber = null;
            }
        }
    }
}