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
    [Register ("AddSpotSizeView")]
    partial class AddSpotSizeView
    {
        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UITableView tableTypes { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnDone != null) {
                btnDone.Dispose ();
                btnDone = null;
            }

            if (tableTypes != null) {
                tableTypes.Dispose ();
                tableTypes = null;
            }
        }
    }
}