using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FlexyPark.UI.Droid
{
    public class Step
    {
        public string Distance { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
        public string Polyline { get; set; }
        public LatLng StartLocation { get; set; }
        public LatLng EndLocation { get; set; }
    }
}