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

    [Activity(Label = "ExtendParkingTimeConfirmView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, LaunchMode = LaunchMode.SingleTask, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class ExtendParkingTimeConfirmView : BaseView
    {
        #region UI Controls
        private System.Timers.Timer mTimer;
        private MvxSubscriptionToken mTimeMessageToken;

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public new ExtendParkingTimeConfirmViewModel ViewModel
        {
            get { return base.ViewModel as ExtendParkingTimeConfirmViewModel; }
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
            SetContentView(Resource.Layout.ExtendParkingTimeConfirmView);
            //ViewModel.View = this;
            mTimeMessageToken = Mvx.Resolve<IMvxMessenger>().SubscribeOnMainThread<TimeMessage>((TimeMessage message) =>
            {
                ViewModel.TotalParkingTime = message.TimeLeft;
            });
            DecreaseTime();

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvPayWithCredits,
                Resource.Id.tvBuyCredits
            });


        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Elapsed -= OnTimedEvent;
                mTimer.Dispose();
                mTimer = null;
            }
        }
        protected override void OnPause()
        {
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
            base.OnPause();
           
        }
      

        #endregion

        #region Implements

        #endregion

        #region Methods

        public void DecreaseTime()
        {
            if (mTimer == null)
            {
                mTimer = new System.Timers.Timer(1000);
                mTimer.Elapsed += OnTimedEvent;
                mTimer.Enabled = true;
            }


        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            //ViewModel.BookingTime--;

            ////Update visual representation here
            ////Remember to do it on UI thread

            //if (ViewModel.BookingTime == 0)
            //{
            //    mTimer.Stop();
            //}
        }

        public void StopTimer()
        {
            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Elapsed -= OnTimedEvent;
                mTimer.Dispose();
                mTimer = null;
            }
        }

        #endregion

       
      
    }
}