using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using Java.Util;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(Label = "BookingView", ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class BookingView : BaseView
    {
        #region UI Controls



        #endregion

        #region Variables

        private System.Timers.Timer mTimer;

        #endregion

        #region Overrides

        public new BookingViewModel ViewModel
        {
            get { return base.ViewModel as BookingViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.BookingView);
            //ViewModel.View = this;
            DecreaseTime();
            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.llVehicle,
                Resource.Id.tvBuyCredits,
                Resource.Id.tvPayNow,
                
               
            });

        }

        protected async override void OnResume()
        {
            base.OnResume();
       
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
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

        #endregion

        #region Implements

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

        #endregion


    }
}