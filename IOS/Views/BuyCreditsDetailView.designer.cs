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
    [Register ("BuyCreditsDetailView")]
    partial class BuyCreditsDetailView
    {
        [Outlet]
        UIKit.UIButton btnBuy { get; set; }


        [Outlet]
        UIKit.UILabel lbDescription { get; set; }


        [Outlet]
        UIKit.UILabel lbPrice { get; set; }


        [Outlet]
        UIKit.UILabel lbTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnBuy != null) {
                btnBuy.Dispose ();
                btnBuy = null;
            }

            if (lbDescription != null) {
                lbDescription.Dispose ();
                lbDescription = null;
            }

            if (lbPrice != null) {
                lbPrice.Dispose ();
                lbPrice = null;
            }

            if (lbTitle != null) {
                lbTitle.Dispose ();
                lbTitle = null;
            }
        }
    }
}