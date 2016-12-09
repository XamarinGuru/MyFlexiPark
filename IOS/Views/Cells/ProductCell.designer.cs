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
    [Register ("ProductCell")]
    partial class ProductCell
    {
        [Outlet]
        UIKit.UILabel lbName { get; set; }


        [Outlet]
        UIKit.UILabel lbPrice { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbName != null) {
                lbName.Dispose ();
                lbName = null;
            }

            if (lbPrice != null) {
                lbPrice.Dispose ();
                lbPrice = null;
            }
        }
    }
}