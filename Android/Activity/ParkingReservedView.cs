using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using FlexyPark.UI.Droid.Services;
using FragmentManager = Android.App.FragmentManager;

namespace FlexyPark.UI.Droid.Activity
{

    [Activity(Label = "ParkingReservedView", MainLauncher = false, ScreenOrientation = ScreenOrientation.SensorPortrait, LaunchMode = LaunchMode.SingleTask, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class ParkingReservedView : BaseView, IParkingReservedView
    {
        #region UI Controls

        private TextViewWithFont tvTitle;
        private ViewPager mViewPager;
        private TextView tvOverview, tvStart;
        private string[] TextOverview;
        private ImageView ivExtendParkingTime;

        private MediaPlayer mp;

        #endregion

        #region Variables

        public static string[] CONTENT;


        private System.Timers.Timer mTimer;
        private LocationManager lm;
        private bool GPSEnable = false;
        AlertDialog dialog;

        #endregion

        #region Constructors

        public new ParkingReservedViewModel ViewModel
        {
            get { return base.ViewModel as ParkingReservedViewModel; }
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
            SetContentView(Resource.Layout.ParkingReservedView);
            ViewModel.View = this;
            CONTENT = new string[] { Mvx.Resolve<ICacheService>().TextSource.GetText("SummaryText"), Mvx.Resolve<ICacheService>().TextSource.GetText("MapText") };
            TextOverview = new[]
            {
                Mvx.Resolve<ICacheService>().TextSource.GetText("OverviewText"),
                Mvx.Resolve<ICacheService>().TextSource.GetText("ResumeText")
            };
            Mvx.Resolve<IFixMvvmCross>().ParkingReservedViewModel = this.ViewModel;
            tvOverview = FindViewById<TextView>(Resource.Id.tvOverview);
            tvOverview.Text = TextOverview[0];
            tvStart = FindViewById<TextView>(Resource.Id.tvStart);
            ivExtendParkingTime = FindViewById<ImageView>(Resource.Id.ivExtendParkingTime);

            tvOverview.Click += (sender, args) =>
            {


                tvOverview.Text = ViewModel.OverviewResumeTitle;

            };


            tvTitle = FindViewById<TextViewWithFont>(Resource.Id.tvfTitle);
            mViewPager = FindViewById<ViewPager>(Resource.Id.viewPager);
            mViewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager);
            mViewPager.PageSelected += (sender, args) =>
            {
                if (args.Position == 1)
                {
                    GPSEnable = false;
                    lm = (LocationManager)GetSystemService(LocationService);

                    try
                    {
                        GPSEnable = lm.IsProviderEnabled(LocationManager.GpsProvider);
                    }
                    catch (Exception ex)
                    {

                    }
                    if (!GPSEnable)
                    {

                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        dialog = builder.Create();
                        dialog.SetTitle(ViewModel.SharedTextSource.GetText("WarningText"));
                        dialog.SetMessage(ViewModel.SharedTextSource.GetText("TurnOnGPSText"));
                        dialog.SetButton(ViewModel.SharedTextSource.GetText("CancelText"), (o, eventArgs) => dialog.Dismiss());
                        dialog.SetButton2(ViewModel.SharedTextSource.GetText("SettingText"), (o, eventArgs) =>
                        {
                            StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                        });

                        dialog.Show();
                    }
                    if (ViewModel.MapVM.HasStaredNavigation)
                    {
                        ivExtendParkingTime.Visibility = ViewStates.Invisible;
                        tvOverview.Visibility = ViewStates.Visible;
                        tvStart.Visibility = ViewStates.Gone;

                        if (ViewModel.MapVM.IsNavigating)
                        {

                        }
                    }
                    else
                    {
                        ivExtendParkingTime.Visibility = ViewStates.Invisible;
                        tvOverview.Visibility = ViewStates.Gone;
                        tvStart.Visibility = ViewStates.Visible;
                    }


                }
                else
                {
                    if (args.Position == 0)
                    {
                        ivExtendParkingTime.Visibility = ViewStates.Visible;
                        tvOverview.Visibility = ViewStates.Gone;
                        tvStart.Visibility = ViewStates.Gone;
                    }
                }
            };
            /*TabPageIndicator mIndicator = FindViewById<TabPageIndicator>(Resource.Id.indicator);
            mIndicator.SetViewPager(mViewPager);*/

            if (ViewModel.Status == ParkingStatus.Rented)
            {
                DecreaseTime();
            }
            else
            {
                tvTitle.Text = ViewModel.SummaryVM.StartTime.ToString("d");
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopTimer();
        }

        protected override void OnResume()
        {
            base.OnResume();

            ivExtendParkingTime = FindViewById<ImageView>(Resource.Id.ivExtendParkingTime);
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

        public void ShowMapTab()
        {
            mViewPager.SetCurrentItem(1, true);
        }

        public void Buzzing(bool isStart)
        {//:play tone;
            //if (isStart)
            //{
            //    try
            //    {
            //        if (mp != null)
            //        {
            //            if (mp.IsPlaying)
            //            {
            //                mp.Stop();
            //                mp.Release();

            //            }
            //        }

            //        mp = MediaPlayer.Create(this, Resource.Raw.doorbell);
            //        mp.Start();
            //        mp.Looping = true;

            //    }
            //    catch (Exception ex)
            //    {

            //        throw;
            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        if (mp != null)
            //        {
            //            if (mp.IsPlaying)
            //            {
            //                mp.Stop();
            //                mp.Release();

            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //    }


            //}


        }

        public void ChangeBarButton(bool isNavigating)
        {
            if (ViewModel.MapVM.IsDrawedStreet)
            {
                if (isNavigating)
                {
                    tvOverview.Visibility = ViewStates.Visible;
                    ViewModel.OverviewResumeTitle = TextOverview[0];
                    tvStart.Visibility = ViewStates.Gone;
                    ivExtendParkingTime.Visibility = ViewStates.Gone;
                }
                else
                {
                    tvOverview.Visibility = ViewStates.Gone;
                    ivExtendParkingTime.Visibility = ViewStates.Gone;
                    tvStart.Visibility = ViewStates.Visible;
                }
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
            ViewModel.TotalParkingTime--;

            //Update visual representation here
            //Remember to do it on UI thread

            if (ViewModel.TotalParkingTime == 0)
            {
                mTimer.Stop();
            }
        }

        #region Adapter


        private class ViewPagerAdapter : FragmentPagerAdapter, TitleProvider
        {
            private const int MaxCount = 1;

            public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm)
                : base(fm)
            {
            }

            public override int Count
            {
                get
                {
                    return MaxCount;
                }
            }
            public string GetTitle(int position)
            {
                return CONTENT[position % CONTENT.Length].ToUpper();
            }
            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                BaseFragment fragment;

                switch (position)
                {
                    case 0:     // Basic Search
                        fragment = new ParkingSummaryFragment();
                        break;

                    default:
                        //fragment = new ParkingMapFragment();
                        fragment = new ParkingSummaryFragment();
                        break;
                }

                return fragment;
            }

        }
        #endregion

        #endregion


    }
}