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
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.ViewModels;

namespace FlexyPark.UI.Droid.Activity
{


    [Activity(Label = "ExtendParkingTimeView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, LaunchMode = LaunchMode.SingleTask, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class ExtendParkingTimeView : BaseView
    {
        #region UI Controls

        private MvxSubscriptionToken mTimeMessageToken;

        #endregion

        #region Variables


        #endregion

        #region Constructors

        public new ExtendParkingTimeViewModel ViewModel
        {
            get { return base.ViewModel as ExtendParkingTimeViewModel; }
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
            SetContentView(Resource.Layout.ExtendParkingTimeView);
            mTimeMessageToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<TimeMessage>((TimeMessage message) =>
            {
                ViewModel.TotalParkingTime = message.TimeLeft;
            });

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack
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