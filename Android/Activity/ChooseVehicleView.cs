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
    [Activity(Label = "ChooseVehicle", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class ChooseVehicleView : BaseView, IChooseVehicleView
    {
        #region UI Controls

        private TextNeueBold tvEdit;

        #endregion

        #region Variables

        #endregion

        #region Constructors

        #endregion

        #region Overrides

        public new ChooseVehicleViewModel ViewModel
        {
            get { return base.ViewModel as ChooseVehicleViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ChooseVehicleView);
            ViewModel.View = this;
            tvEdit = FindViewById<TextNeueBold>(Resource.Id.tvEdit);

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.ivAdd,
                Resource.Id.tvEdit
            });



        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        protected override void OnStart()
        {
            base.OnStart();
            ViewModel.GetVehicles();
        }


        protected override void OnResume()
        {
            base.OnResume();
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