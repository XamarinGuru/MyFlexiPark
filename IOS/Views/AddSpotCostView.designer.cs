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
    [Register ("AddSpotCostView")]
    partial class AddSpotCostView
    {
        [Outlet]
        UIKit.UILabel lbRecommendedPrice { get; set; }


        [Outlet]
        UIKit.UILabel lbRecomPrice { get; set; }


        [Outlet]
        UIKit.UILabel lbSelectedPrice { get; set; }


        [Outlet]
        UIKit.UILabel lbSelePrice { get; set; }


        [Outlet]
        UIKit.UISlider sdTime { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbRecommendedPrice != null) {
                lbRecommendedPrice.Dispose ();
                lbRecommendedPrice = null;
            }

            if (lbRecomPrice != null) {
                lbRecomPrice.Dispose ();
                lbRecomPrice = null;
            }

            if (lbSelectedPrice != null) {
                lbSelectedPrice.Dispose ();
                lbSelectedPrice = null;
            }

            if (lbSelePrice != null) {
                lbSelePrice.Dispose ();
                lbSelePrice = null;
            }

            if (sdTime != null) {
                sdTime.Dispose ();
                sdTime = null;
            }
        }
    }
}