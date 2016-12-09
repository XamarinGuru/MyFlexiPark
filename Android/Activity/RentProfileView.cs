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
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;

namespace FlexyPark.UI.Droid.Activity
{
	[Activity(Label = "RentProfileView", MainLauncher = false,
	  WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, ScreenOrientation = ScreenOrientation.SensorPortrait, Theme = "@style/AppBaseTheme")]
	public class RentProfileView : BaseView, IRentProfileView, DatePickerDialog.IOnDateSetListener
	{
		#region UI Controls

		private TextRegular tvPick;

		private TextRegular btValidity;

		#endregion

		#region Variables

		#endregion

		#region Constructors

		public new RentProfileViewModel ViewModel
		{
			get { return base.ViewModel as RentProfileViewModel; }
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
			SetContentView(Resource.Layout.RentProfileView);
			ViewModel.View = this;

			//this.EnsureBindingContextIsSet(savedInstanceState);
			Init();

			SetButtonEffects(new List<int>()
			{
			   Resource.Id.tvPick,
			   Resource.Id.tvVehicleManagement
			});

		}

		protected override void OnPause()
		{
			base.OnPause();
			OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
		}

		#endregion

		#region Implements

		public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
		{
			string month = (monthOfYear + 1).ToString("00");

			btValidity.Text = month + "/" + year;

		}

		public void ShowPicker()
		{
			//InitPicker();

			//picker.Select((int)ViewModel.ValidityTime.Month - 1, 0, false);
			//picker.Select((int)ViewModel.ValidityTime.Year - DateTime.Today.Year, 1, false);

			//popoverPicker.PresentPopoverFromRect(btnPick.Frame, vContent, WYPopoverArrowDirection.Down, true);
		}

		public void ShowCreditPicker()
		{
			//InitCreditPicker();
			//creditPicker.Select(ViewModel.SelectedCredit, 0, false);
			//popoverCreditPicker.PresentPopoverFromRect(btnCreditPick.Frame, vContent, WYPopoverArrowDirection.Down, true);
		}

		public void CloseKeyboard()
		{
			//View.EndEditing(true);
		}
		//public void SetModeTitle(string title)
		//{
		//	tvEdit.Text = title;
		//}

		#endregion

		#region Methods

		public void Init()
		{

			btValidity = FindViewById<TextRegular>(Resource.Id.tvValidity);
			tvPick = FindViewById<TextRegular>(Resource.Id.tvPick);

			DatePickerDialog mDatePickerDialog = new DatePickerDialog(this, this, ViewModel.ValidityTime.Year, ViewModel.ValidityTime.Month - 1, ViewModel.ValidityTime.Day);

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
