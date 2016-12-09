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
    [Register ("VehicleCell")]
    partial class VehicleCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnEdit { get; set; }


        [Outlet]
        UIKit.NSLayoutConstraint cstVehicleTypeTrailing { get; set; }


        [Outlet]
        UIKit.UILabel lbPlateNumber { get; set; }


        [Outlet]
        UIKit.UILabel lbVehicleType { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnDelete != null) {
                btnDelete.Dispose ();
                btnDelete = null;
            }

            if (btnEdit != null) {
                btnEdit.Dispose ();
                btnEdit = null;
            }

            if (cstVehicleTypeTrailing != null) {
                cstVehicleTypeTrailing.Dispose ();
                cstVehicleTypeTrailing = null;
            }

            if (lbPlateNumber != null) {
                lbPlateNumber.Dispose ();
                lbPlateNumber = null;
            }

            if (lbVehicleType != null) {
                lbVehicleType.Dispose ();
                lbVehicleType = null;
            }
        }
    }
}