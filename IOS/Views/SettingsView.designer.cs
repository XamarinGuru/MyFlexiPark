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
    [Register ("SettingsView")]
    partial class SettingsView
    {
        [Outlet]
        UIKit.UIButton btnAppSettings { get; set; }


        [Outlet]
        UIKit.UIButton btnMyProfile { get; set; }


        [Outlet]
        UIKit.UIButton btnMyVehicles { get; set; }


        [Outlet]
        UIKit.UIImageView ivBackground { get; set; }


        [Outlet]
        UIKit.UILabel lbAppSettings { get; set; }


        [Outlet]
        UIKit.UILabel lbMyProfile { get; set; }


        [Outlet]
        UIKit.UILabel lbMyVehicles { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnAppSettings != null) {
                btnAppSettings.Dispose ();
                btnAppSettings = null;
            }

            if (btnMyProfile != null) {
                btnMyProfile.Dispose ();
                btnMyProfile = null;
            }

            if (btnMyVehicles != null) {
                btnMyVehicles.Dispose ();
                btnMyVehicles = null;
            }

            if (ivBackground != null) {
                ivBackground.Dispose ();
                ivBackground = null;
            }

            if (lbAppSettings != null) {
                lbAppSettings.Dispose ();
                lbAppSettings = null;
            }

            if (lbMyProfile != null) {
                lbMyProfile.Dispose ();
                lbMyProfile = null;
            }

            if (lbMyVehicles != null) {
                lbMyVehicles.Dispose ();
                lbMyVehicles = null;
            }
        }
    }
}