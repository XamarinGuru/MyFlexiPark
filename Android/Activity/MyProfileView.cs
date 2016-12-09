using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using FlexyPark.UI.Droid.Services;
using Java.Lang;
using FragmentManager = Android.Support.V4.App.FragmentManager;


namespace FlexyPark.UI.Droid.Activity
{
    /// <summary>
    /// </summary>
    [Activity(MainLauncher = false,
        WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.SensorPortrait, Theme = "@style/AppBaseTheme")]
    public class MyProfileView : BaseView, IMyProfileView
    {
        #region UI Controls

        private NonSwipeableViewPager mViewPager;
        private IndicatorControlClick mIndicator;
        private TextViewWithFont tvTitle;

        #endregion

        #region Variables

        public static string[] CONTENT, Title;



        #endregion

        #region Constructors


        #endregion

        #region Overrides

        public new MyProfileViewModel ViewModel
        {
            get { return base.ViewModel as MyProfileViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();
            Mvx.Resolve<IFixMvvmCross>().MyProfileViewModel = ViewModel;

        }

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MyProfileView);
            ViewModel.View = this;


            CONTENT = new string[] { ViewModel.TextSource.GetText("CommonText"), ViewModel.TextSource.GetText("RentText"), ViewModel.TextSource.GetText("OwnText") };
            Title = new string[] { ViewModel.TextSource.GetText("MyProfileText"), ViewModel.TextSource.GetText("PaymentConfigurationText"), ViewModel.TextSource.GetText("OwnText") };

            tvTitle = FindViewById<TextViewWithFont>(Resource.Id.title);
            mViewPager = FindViewById<NonSwipeableViewPager>(Resource.Id.viewPager);
            mViewPager.Adapter = new ViewPagerAdapter(SupportFragmentManager);
            mIndicator = FindViewById<IndicatorControlClick>(Resource.Id.indicator);
            mIndicator.SetViewPager(mViewPager);

            mViewPager.PageSelected += (sender, args) =>
            {
                mIndicator.SetCurrentItem(args.Position);
                SetTitle(args.Position);
            };
            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack
            });





        }



        #endregion

        #region Implements

        public void ShowTab(int tabIndex)
        {
            if (mViewPager != null)
            {
                mViewPager.SetCurrentItem(tabIndex, true);

            }
            SetTitle(tabIndex);

            if (!ViewModel.IsAllowTabChange)
            {
                mViewPager.SetPagingEnabled(false);
                mIndicator.setClickable(false);



            }


        }


        #endregion

        #region Methods

        public void SetTitle(int titleId)
        {

            switch (titleId)
            {
                case 0:
                    tvTitle.Text = Title[titleId];
                    break;
                case 1:
                    tvTitle.Text = Title[titleId];
               
                    
                    break;
                case 2:
                    tvTitle.Text = Title[titleId];
                    break;


            }
        }

        #region Adapter


        private class ViewPagerAdapter : FragmentPagerAdapter, TitleProvider
        {
            private const int MaxCount = 3;


            public ViewPagerAdapter(FragmentManager fm)
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
                return MyProfileView.CONTENT[position % MyProfileView.CONTENT.Length].ToUpper();
            }

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {


                {
                    BaseFragment fragment;

                    switch (position)
                    {
                        case 0: // Basic Search
                            fragment = new CommonFragment();
                            break;
                        //case 1: // Basic Search
                        //    fragment = new RentFragment();
                        //    break;

                        default: // Advanced Search
                            fragment = new OwnFragment();
                            break;
                    }

                    return fragment;
                }


            }






        }

        #endregion



        #region Indicator


        #endregion

        #endregion

    }
}
