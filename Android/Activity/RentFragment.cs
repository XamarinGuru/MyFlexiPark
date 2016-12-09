using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.Versioning;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using FlexyPark.UI.Droid.Services;
using Java.Lang.Reflect;

namespace FlexyPark.UI.Droid.Activity
{
    public class RentFragment : BaseFragment, DatePickerDialog.IOnDateSetListener
    {
        #region UI Controls

        private TextRegular tvPick;

        private TextRegular btValidity;

        #endregion

        #region Variables

        #endregion

        #region Constructors

        public RentFragment()
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().MyProfileViewModel.RentVM;
        }

		public new RentFragmentViewModel ViewModel
        {
            get { return base.ViewModel as RentFragmentViewModel; }
            set
            {
                base.ViewModel = value;

            }
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.RentFragment, container, false);
            Init(view);

            SetButtonEffects(view, new List<int>()
            {
               Resource.Id.tvPick,
               Resource.Id.tvVehicleManagement
            });
            return view;
        }

        #endregion

        #region Implements

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            string month = (monthOfYear + 1).ToString("00");

            btValidity.Text = month + "/" + year;

        }

        #endregion

        #region Methods

        public void Init(View view)
        {

            btValidity = view.FindViewById<TextRegular>(Resource.Id.tvValidity);
            tvPick = view.FindViewById<TextRegular>(Resource.Id.tvPick);

            DatePickerDialog mDatePickerDialog = new DatePickerDialog(view.Context, this, ViewModel.ValidityTime.Year, ViewModel.ValidityTime.Month - 1, ViewModel.ValidityTime.Day);

            mDatePickerDialog.DatePicker.SpinnersShown = true;
           
            

            //if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            //{
            //    int dayPickerId = Android.Content.Res.Resources.System.GetIdentifier("day", "id", "android");
            //    if (dayPickerId != 0)
            //    {
            //        var dayView = mDatePickerDialog.FindViewById(dayPickerId);
            //        if (dayView != null)
            //        {
            //            dayView.Visibility = ViewStates.Gone;
            //        }

            //    }
            //}
            //else
            {

                if (
                    ((ViewGroup)mDatePickerDialog.DatePicker).FindViewById(
                        Android.Content.Res.Resources.System.GetIdentifier("day", "id", "android")) != null)
                {
                    ((ViewGroup)mDatePickerDialog.DatePicker).FindViewById(
                        Android.Content.Res.Resources.System.GetIdentifier("day", "id", "android")).Visibility =
                        ViewStates.Gone;
                }
            }




            btValidity.Click += (sender, args) =>
            {
                mDatePickerDialog.Show();

            };
            tvPick.Click += (sender, args) =>
            {
                mDatePickerDialog.Show();

            };


        }

        #endregion


    }
}