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
    [Register ("VehicleTypeCell")]
    partial class VehicleTypeCell
    {
        [Outlet]
        UIKit.UILabel lbType { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbType != null) {
                lbType.Dispose ();
                lbType = null;
            }
        }
    }
}