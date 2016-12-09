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
using Cirrious.MvvmCross.Binding.Droid.Views;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class ReportSelectionView : BaseView
    {
        #region	UI Controls

        #endregion

        #region	Constructors

        public ReportSelectionViewModel ViewMode
        {
            get { return base.ViewModel as ReportSelectionViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region	Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ReportSelectionView);
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        #endregion

        #region	Implements

        #endregion

        #region	Methods

        #region Init

        private void Init()
        {
        }

        #endregion

        #endregion
    }
}