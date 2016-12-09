using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.Views;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Services;

namespace FlexyPark.UI.Droid.Activity
{
    public class AddSpotCalendarTabMonth : BaseFragment, IAddSpotCalendarView
    {
        #region UI Controls

        private ViewPager viewPager;
        private ImageView CalendarNext, CalendarBack;
        private MvxListView mMvxListView;



        #endregion

        #region Variables

        public AddSpotCalendarViewModel AddSpotCalendarVM = null;
        private int mState;

        #endregion

        #region Constructors

        public AddSpotCalendarTabMonth()
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().AddSpotCalendarViewModel;
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            this.EnsureBindingContextIsSet(savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.AddSpotCalendarTabMonth, container, false);

            AddSpotCalendarVM = ViewModel as AddSpotCalendarViewModel;
            AddSpotCalendarVM.View = this;

            mMvxListView = view.FindViewById<MvxListView>(Resource.Id.MvxListView);

            CalendarBack = view.FindViewById<ImageView>(Resource.Id.ivCalendarBack);
            CalendarNext = view.FindViewById<ImageView>(Resource.Id.ivCalendarNext);
            viewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager);
            CalendarBack.Click += (sender, args) =>
            {
                int index = viewPager.CurrentItem - 1;
                viewPager.SetCurrentItem(--viewPager.CurrentItem, false);
            };
            CalendarNext.Click += (sender, args) =>
            {
                int index = viewPager.CurrentItem + 1;
                viewPager.SetCurrentItem(++viewPager.CurrentItem, false);
            };

            // re Init when change tab
            viewPager.Adapter = new ViewPagerAdapter(ChildFragmentManager);

            viewPager.SetCurrentItem(AddSpotCalendarVM.PagerSelected, false);

            SetButtonEffects(view, new List<int>()
            {
                Resource.Id.ivCalendarNext,
                Resource.Id.ivCalendarBack
            });

            return view;

        }

        public override void OnPause()
        {
            base.OnPause();

            if (viewPager != null)
            {
                AddSpotCalendarVM.PagerSelected = viewPager.CurrentItem;
            }

        }

        #endregion

        #region Implements

        public void SetModeTitle(string title)
        {
        }

        public void ReloadTable()
        {
        }

        public void UpdateCalendar(bool forceReload = false)
        {
            viewPager.Adapter = new ViewPagerAdapter(ChildFragmentManager);
            viewPager.SetCurrentItem(AddSpotCalendarVM.PagerSelected, true);
            if (AddSpotCalendarVM.AddSpotCalendarDayView != null)
            {
                AddSpotCalendarVM.AddSpotCalendarDayView.UpdateCalendar(forceReload);
            }
        }



        public void GotoToday()
        {
            if (viewPager != null)
            {
                viewPager.SetCurrentItem(100, false);
            }
        }



        #endregion

        #region Methods

        #region Adpater ViewPager

        private class ViewPagerAdapter : FragmentPagerAdapter
        {
            private int MaxCount = 200;


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

            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                int month = position - 100;
                MvxFragment fragment;

                fragment = new ItemViewPagerCalendarFragment(month);


                return fragment;
            }


        }

        #endregion


        #endregion


    }
}