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
    [Register ("AddSpotView")]
    partial class AddSpotView
    {
        [Outlet]
        UIKit.UIButton btnAddSpot { get; set; }


        [Outlet]
        UIKit.UILabel lbHorizontalAccuracy { get; set; }


        [Outlet]
        UIKit.UITableView tableTasks { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tableTasks != null) {
                tableTasks.Dispose ();
                tableTasks = null;
            }
        }
    }
}