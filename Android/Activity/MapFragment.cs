using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FlexyPark.UI.Droid.Activity
{
    public class MapFragment : SupportMapFragment
    {
        public override void OnResume()
        {
            base.OnResume();

            Map.MyLocationEnabled = false;
            Map.MapType = GoogleMap.MapTypeNormal;
        }
    }
}