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

    [Activity(Label = "MyReservationsView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class MyReservationsView : BaseView
    {
        #region UI Controls

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public new MyReservationsViewModel ViewModel
        {
            get { return base.ViewModel as MyReservationsViewModel; }
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
            SetContentView(Resource.Layout.MyReservationsView);

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
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
            ViewModel.GetMyReservations();
        }

        #endregion

        #region Implements

        #endregion

        #region Methods

        #endregion
    }
}