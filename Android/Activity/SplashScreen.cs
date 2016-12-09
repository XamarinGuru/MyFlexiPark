using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Droid.Views;
using Com.Crittercism.App;
using FlexyPark.Core;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "FlexyPark", MainLauncher = true, Icon = "@drawable/icon", NoHistory = true, ScreenOrientation = ScreenOrientation.SensorPortrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
            Crittercism.Initialize(Application.Context, "73352a1aafd3428f8805f8bb621b13c100444503");
			Stripe.StripeClient.DefaultPublishableKey = AppConstants.StripeAPIKey;
        }
    }
}