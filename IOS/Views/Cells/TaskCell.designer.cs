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
    [Register ("TaskCell")]
    partial class TaskCell
    {
        [Outlet]
        UIKit.UIImageView ivIcon { get; set; }


        [Outlet]
        UIKit.UILabel lbPleaseWait { get; set; }


        [Outlet]
        UIKit.UILabel lbTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ivIcon != null) {
                ivIcon.Dispose ();
                ivIcon = null;
            }

            if (lbPleaseWait != null) {
                lbPleaseWait.Dispose ();
                lbPleaseWait = null;
            }

            if (lbTitle != null) {
                lbTitle.Dispose ();
                lbTitle = null;
            }
        }
    }
}