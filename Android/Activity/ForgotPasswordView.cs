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
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "Menu View", MainLauncher = false, LaunchMode = LaunchMode.SingleTask,
    WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize,
    ScreenOrientation = ScreenOrientation.SensorPortrait, Theme = "@style/AppBaseTheme")]
    public class ForgotPasswordView : BaseView
    {
        #region	UIControls

        #endregion

        #region	Variables

        #endregion

        #region	Overrides

        public new LostPasswordViewModel ViewModel
        {
            get { return base.ViewModel as LostPasswordViewModel; }
            set { base.ViewModel = value; }
        }


        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ForgotPasswordView);
            SetButtonEffects(new List<int>()
            {
                Resource.Id.tvForgotPassword,
                Resource.Id.rlBack
            });

        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
           
        }

        #endregion

        #region	Implement

        #endregion

        #region	Methods

        #endregion
    }
}