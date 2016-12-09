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
    [Register ("AddSpotStatusView")]
    partial class AddSpotStatusView
    {
        [Outlet]
        UIKit.UISwitch swStatus { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (swStatus != null) {
                swStatus.Dispose ();
                swStatus = null;
            }
        }
    }
}