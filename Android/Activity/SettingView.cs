using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
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
    [Activity(Label = "Setting", MainLauncher = false, ScreenOrientation = ScreenOrientation.SensorPortrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class SettingView : BaseView
    {
        #region UI Controls

        #endregion

        #region Variables

        #endregion

        #region Overrides

        public new SettingsViewModel ViewModel
        {
            get { return base.ViewModel as SettingsViewModel; }
            set
            {
                base.ViewModel = value;
            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SettingView);
            SetButtonEffects(new List<int>()
            {
                Resource.Id.llMyProfile,
                Resource.Id.llSetting,
                Resource.Id.rlBack,
            });
        }

        protected override void OnResume()
        {
            base.OnResume();
           
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        #endregion

        #region Implements

        #endregion

        #region Methods

        #endregion
    }
}