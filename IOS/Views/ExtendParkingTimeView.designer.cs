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
    [Register ("ExtendParkingTimeView")]
    partial class ExtendParkingTimeView
    {
        [Outlet]
        UIKit.UITableView tableExtendTime { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tableExtendTime != null) {
                tableExtendTime.Dispose ();
                tableExtendTime = null;
            }
        }
    }
}