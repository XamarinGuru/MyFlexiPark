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
    [Register ("AppSettingsView")]
    partial class AppSettingsView
    {
        [Outlet]
        UIKit.UIButton btnPickLanguage { get; set; }


        [Outlet]
        UIKit.UILabel lbAppLang { get; set; }


        [Outlet]
        UIKit.UILabel lbAppVer { get; set; }


        [Outlet]
        UIKit.UILabel lbAppVersion { get; set; }


        [Outlet]
        UIKit.UILabel lbUseGG { get; set; }


        [Outlet]
        UIKit.UILabel lbUseNavmii { get; set; }


        [Outlet]
        UIKit.UILabel lbUseWaze { get; set; }


        [Outlet]
        UIKit.UISwitch swGoogle { get; set; }


        [Outlet]
        UIKit.UISwitch swNavmii { get; set; }


        [Outlet]
        UIKit.UISwitch swWaze { get; set; }


        [Outlet]
        UIKit.UITextField tfLanguage { get; set; }


        [Outlet]
        UIKit.UIView vContent { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnPickLanguage != null) {
                btnPickLanguage.Dispose ();
                btnPickLanguage = null;
            }

            if (lbAppLang != null) {
                lbAppLang.Dispose ();
                lbAppLang = null;
            }

            if (lbAppVer != null) {
                lbAppVer.Dispose ();
                lbAppVer = null;
            }

            if (lbAppVersion != null) {
                lbAppVersion.Dispose ();
                lbAppVersion = null;
            }

            if (lbUseGG != null) {
                lbUseGG.Dispose ();
                lbUseGG = null;
            }

            if (lbUseNavmii != null) {
                lbUseNavmii.Dispose ();
                lbUseNavmii = null;
            }

            if (lbUseWaze != null) {
                lbUseWaze.Dispose ();
                lbUseWaze = null;
            }

            if (swGoogle != null) {
                swGoogle.Dispose ();
                swGoogle = null;
            }

            if (swNavmii != null) {
                swNavmii.Dispose ();
                swNavmii = null;
            }

            if (swWaze != null) {
                swWaze.Dispose ();
                swWaze = null;
            }

            if (tfLanguage != null) {
                tfLanguage.Dispose ();
                tfLanguage = null;
            }

            if (vContent != null) {
                vContent.Dispose ();
                vContent = null;
            }
        }
    }
}