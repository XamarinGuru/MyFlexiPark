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
    [Register ("RouteCell")]
    partial class RouteCell
    {
        [Outlet]
        UIKit.UILabel lbDistance { get; set; }


        [Outlet]
        UIKit.UILabel lbInstruction { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbDistance != null) {
                lbDistance.Dispose ();
                lbDistance = null;
            }

            if (lbInstruction != null) {
                lbInstruction.Dispose ();
                lbInstruction = null;
            }
        }
    }
}