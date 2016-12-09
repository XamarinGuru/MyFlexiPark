using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using FlexyPark.Core;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using FlexyPark.UI.Droid.Services;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(MainLauncher = false, ScreenOrientation = ScreenOrientation.SensorPortrait, LaunchMode = LaunchMode.SingleTask, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]
    public class AddSpotCalendarTabView : BaseView, IAddSpotCalendarTabView
    {
        #region UI Controls

        private NonSwipeableViewPager mViewPager;

        #endregion

        #region Variables

        public static string[] CONTENT = new string[] { "Day", "Month" };

        #endregion

        #region Constructors

        public new AddSpotCalendarViewModel ViewModel
        {
            get { return base.ViewModel as AddSpotCalendarViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddSpotCalendarTabView);
            Mvx.Resolve<IFixMvvmCross>().AddSpotCalendarViewModel = ViewModel;
            ViewModel.AddSpotCalendarTabView = this;
            mViewPager = FindViewById<NonSwipeableViewPager>(Resource.Id.viewPager);
            mViewPager.SetPagingEnabled(false);
            mViewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager);
            TabPageIndicator mIndicator = FindViewById<TabPageIndicator>(Resource.Id.indicator);
            mIndicator.SetViewPager(mViewPager);

            mViewPager.PageSelected += (sender, args) =>
            {
                if (args.Position == 0)
                {
                    ViewModel.AddSpotCalendarDayView.GotoDateSelected();
                }
            };

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvDone,
                Resource.Id.ivAdd,
                Resource.Id.tvToday
            });

            mViewPager.CurrentItem = 1;

        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
            Mvx.Resolve<ICacheService>().NextStatus = AddSpotStatus.Activation;
        }

        protected override void OnResume()
        {
            base.OnResume();
            ViewModel.GetUnavaiabilities(Core.Helpers.DateTimeHelpers.GetFirstDayOfMonthAndYear(DateTime.UtcNow.Year, DateTime.UtcNow.Month));
        }

        #endregion

        #region Implements

        #endregion

        #region Methods

        #region Adapter


        private class ViewPagerAdapter : FragmentPagerAdapter, TitleProvider
        {
            private const int MaxCount = 2;

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
                    case 1:
                        fragment = new AddSpotCalendarTabMonth();
                        break;

                    default:
                        fragment = new AddSpotCalendarTabDay();

                        break;
                }

                return fragment;
            }

        }
        #endregion

        #endregion

        public void ShowTabDay(DateTime dateTime)
        {

        }

        public void GetUnavaiabilities()
        {
            ViewModel.GetUnavaiabilities(Core.Helpers.DateTimeHelpers.GetFirstDayOfMonthAndYear(DateTime.UtcNow.Year, DateTime.UtcNow.Month));
        }
    }
}