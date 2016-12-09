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
    [Register ("AddSpotCalendarView")]
    partial class AddSpotCalendarView
    {
        [Outlet]
        UIKit.UIButton btnAdd { get; set; }


        [Outlet]
        UIKit.UIButton btnDone { get; set; }


        [Outlet]
        UIKit.UIButton btnEdit { get; set; }


        [Outlet]
        UIKit.UILabel lbDate { get; set; }


        [Outlet]
        UIKit.UITableView tableEvents { get; set; }


        [Outlet]
        UIKit.UIView vCalendar { get; set; }


        [Outlet]
        UIKit.UIView vEventPopup { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAdd != null) {
                btnAdd.Dispose ();
                btnAdd = null;
            }

            if (btnDone != null) {
                btnDone.Dispose ();
                btnDone = null;
            }

            if (btnEdit != null) {
                btnEdit.Dispose ();
                btnEdit = null;
            }

            if (lbDate != null) {
                lbDate.Dispose ();
                lbDate = null;
            }

            if (tableEvents != null) {
                tableEvents.Dispose ();
                tableEvents = null;
            }

            if (vCalendar != null) {
                vCalendar.Dispose ();
                vCalendar = null;
            }

            if (vEventPopup != null) {
                vEventPopup.Dispose ();
                vEventPopup = null;
            }
        }
    }
}