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
    [Activity(Label = "AddSpotAddressView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class AddSpotAddressView : BaseView
    {
        #region UI Controls

        #endregion

        #region Variables

        #endregion

        #region Constructors

        #endregion

        #region Overrides

        public new AddSpotAddressViewModel ViewModel
        {
            get { return base.ViewModel as AddSpotAddressViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddSpotAddressView);

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvDone
            });

        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
            ViewModel.DoneCommand.Execute();
        }

        #endregion

        #region Implements

        #endregion

        #region Methods

        #endregion

    }
}