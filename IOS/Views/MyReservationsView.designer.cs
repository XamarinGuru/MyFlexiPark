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
    [Register ("MyReservationsView")]
    partial class MyReservationsView
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.UITableView tableReservations { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (tableReservations != null) {
                tableReservations.Dispose ();
                tableReservations = null;
            }
        }
    }
}