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
    [Register ("ParkingSlotCell")]
    partial class ParkingSlotCell
    {
        [Outlet]
        UIKit.UIImageView ivClock { get; set; }


        [Outlet]
        UIKit.UILabel lbDistance { get; set; }


        [Outlet]
        UIKit.UILabel lbPrice { get; set; }


        [Outlet]
        UIKit.UILabel lbProvider { get; set; }


        [Outlet]
        UIKit.UIView vRating { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ivClock != null) {
                ivClock.Dispose ();
                ivClock = null;
            }

            if (lbDistance != null) {
                lbDistance.Dispose ();
                lbDistance = null;
            }

            if (lbPrice != null) {
                lbPrice.Dispose ();
                lbPrice = null;
            }

            if (lbProvider != null) {
                lbProvider.Dispose ();
                lbProvider = null;
            }

            if (vRating != null) {
                vRating.Dispose ();
                vRating = null;
            }
        }
    }
}