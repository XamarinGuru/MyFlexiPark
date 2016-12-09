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
    [Register ("EventCell")]
    partial class EventCell
    {
        [Outlet]
        UIKit.UIButton btnDelete { get; set; }


        [Outlet]
        UIKit.UIButton btnEdit { get; set; }


        [Outlet]
        UIKit.UILabel lbEndDate { get; set; }


        [Outlet]
        UIKit.UILabel lbStartDate { get; set; }


        [Outlet]
        UIKit.UILabel lbTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnDelete != null) {
                btnDelete.Dispose ();
                btnDelete = null;
            }

            if (btnEdit != null) {
                btnEdit.Dispose ();
                btnEdit = null;
            }

            if (lbEndDate != null) {
                lbEndDate.Dispose ();
                lbEndDate = null;
            }

            if (lbStartDate != null) {
                lbStartDate.Dispose ();
                lbStartDate = null;
            }

            if (lbTitle != null) {
                lbTitle.Dispose ();
                lbTitle = null;
            }
        }
    }
}