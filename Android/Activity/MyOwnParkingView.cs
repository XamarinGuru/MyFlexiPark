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
using Cirrious.CrossCore;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "MyProfileView", MainLauncher = false,
      WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.SensorPortrait, Theme = "@style/AppBaseTheme")]
    public class MyOwnParkingView : BaseView, IMyOwnParkingView
    {
        #region UI Controls

        private TextNeueBold tvEdit;

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public new MyOwnParkingViewModel ViewModel
        {
            get { return base.ViewModel as MyOwnParkingViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MyOwnParkingView);
            ViewModel.View = this;
            tvEdit = FindViewById<TextNeueBold>(Resource.Id.tvedit);

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvedit,
                Resource.Id.ivAdd
            });

            
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (ViewModel.IsEditMode)
            {
                tvEdit.Text = ViewModel.TextSource.GetText("CancelText");
            }
            else
            {
                tvEdit.Text = ViewModel.TextSource.GetText("DeleteText");
            }

            ViewModel.GetMyOwnParking();
        }

        #endregion

        #region Implements

        public void SetModeTitle(string title)
        {
            tvEdit.Text = title;
        }

        #endregion

        #region Methods

        #endregion


    }
}