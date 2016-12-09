using System;
using UIKit;

namespace FlexyPark.UI.Touch.Helpers
{
    public static class DeviceHelper
    {
        public static bool IsPad
        {
            get
            {
                return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad;
            }
        }
    }
}

