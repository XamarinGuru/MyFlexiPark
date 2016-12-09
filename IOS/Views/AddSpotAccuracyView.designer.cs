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
    [Register ("AddSpotAccuracyView")]
    partial class AddSpotAccuracyView
    {
        [Outlet]
        UIKit.UILabel lbAccuracy { get; set; }


        [Outlet]
        UIKit.UILabel lbCurrentGPSAccuracy { get; set; }


        [Outlet]
        UIKit.UILabel lbCurrentGPSCoor { get; set; }


        [Outlet]
        UIKit.UILabel lbGPSTarget { get; set; }


        [Outlet]
        UIKit.UILabel lbGPSTargetAccuracy { get; set; }


        [Outlet]
        UIKit.UILabel lbIfYourDevice { get; set; }


        [Outlet]
        UIKit.UILabel lbLat { get; set; }


        [Outlet]
        UIKit.UILabel lbLng { get; set; }


        [Outlet]
        UIKit.UILabel lbPleaseWait { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (lbAccuracy != null) {
                lbAccuracy.Dispose ();
                lbAccuracy = null;
            }

            if (lbCurrentGPSAccuracy != null) {
                lbCurrentGPSAccuracy.Dispose ();
                lbCurrentGPSAccuracy = null;
            }

            if (lbCurrentGPSCoor != null) {
                lbCurrentGPSCoor.Dispose ();
                lbCurrentGPSCoor = null;
            }

            if (lbGPSTarget != null) {
                lbGPSTarget.Dispose ();
                lbGPSTarget = null;
            }

            if (lbGPSTargetAccuracy != null) {
                lbGPSTargetAccuracy.Dispose ();
                lbGPSTargetAccuracy = null;
            }

            if (lbIfYourDevice != null) {
                lbIfYourDevice.Dispose ();
                lbIfYourDevice = null;
            }

            if (lbLat != null) {
                lbLat.Dispose ();
                lbLat = null;
            }

            if (lbLng != null) {
                lbLng.Dispose ();
                lbLng = null;
            }

            if (lbPleaseWait != null) {
                lbPleaseWait.Dispose ();
                lbPleaseWait = null;
            }
        }
    }
}