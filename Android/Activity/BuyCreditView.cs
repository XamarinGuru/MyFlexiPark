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

    [Activity(Label = "Setting", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class BuyCreditView : BaseView
    {
        #region UI Controls

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public new BuyCreditsViewModel ViewModel
        {
            get { return base.ViewModel as BuyCreditsViewModel; }
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
            SetContentView(Resource.Layout.BuyCreditView);
            SetButtonEffects(new List<int>()
            {
                //Resource.Id.rlBack,
                //Resource.Id.tv10,
                //Resource.Id.tv25,
                //Resource.Id.tv50,
                //Resource.Id.tv100,
            });

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