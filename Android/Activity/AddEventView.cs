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
using Cirrious.MvvmCross.Binding.BindingContext;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls;
using FlexyPark.UI.Droid.Services;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Util;
using CalendarService = Google.Apis.Calendar.v3.CalendarService;

namespace FlexyPark.UI.Droid.Activity
{
    [Activity(ScreenOrientation = ScreenOrientation.SensorPortrait, MainLauncher = false, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize, Theme = "@style/AppBaseTheme")]

	public class AddEventView : BaseView, IAddEventView, TimePickerDialog.IOnTimeSetListener, DatePickerDialog.IOnDateSetListener
    {
        #region UIControls

        private DatePickerDialog mDatePickerDialog;
        private TimePickerDialog mTimePickerDialog;
        private bool IsStart = true;
        private TextView tvStartDate, tvStartTime, tvEndDate, tvEndTime, tvAdd, tvTitle;
        private CalendarService mCalendarService;

        #endregion

        #region Variable

        public AddEventViewModel ViewModel
        {
            get
            {
                return base.ViewModel as AddEventViewModel;

            }
            set { base.ViewModel = value; }
        }


        #endregion

        #region Overrides

        protected override void OnCreate(Bundle bundle)
        {
            OverridePendingTransition(Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AddEventView);
            ViewModel.View = this;
            Init();

            if (ViewModel.IsReadOnly)
            {
                FindViewById<TextViewWithFont>(Resource.Id.tvTitle).Text = ViewModel.TextSource.GetText("BookingTitle");
            }
            else
            {
                FindViewById<TextViewWithFont>(Resource.Id.tvTitle).Text = ViewModel.TextSource.GetText("UnavailabilityTitle");
            }

            var set = this.CreateBindingSet<AddEventView, AddEventViewModel>();
            set.Bind(tvAdd).For(v => v.Text).To(vm => vm.ButtonTitle).WithConversion("AddEditButtonTitleConverter", ViewModel.ButtonTitle);
            set.Apply();

            SetButtonEffects(new List<int>()
            {
                Resource.Id.rlBack,
                Resource.Id.tvAdd
            });

            FindViewById<FrameLayout>(Resource.Id.flReadOnly).Click += (sender, args) => { };

            mCalendarService = Mvx.Resolve<ICalendarService>().CachedCalendarService;
        }

        #endregion

        protected override void OnPause()
        {
            base.OnPause();
            OverridePendingTransition(Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);
        }

        #region Implements

        public void ShowStartDatePicker()
        {
            IsStart = true;
            mDatePickerDialog = new DatePickerDialog(this, this, ViewModel.StartDate.Year, ViewModel.StartDate.Month - 1, ViewModel.StartDate.Day);
            mDatePickerDialog.Show();
        }

        public void ShowStartTimePicker()
        {
            IsStart = true;
            mTimePickerDialog = new TimePickerDialog(this, this, ViewModel.StartDate.Hour, ViewModel.StartDate.Minute, true);
            mTimePickerDialog.Show();
        }

        public void ShowEndDatePicker()
        {
            IsStart = false;
            mDatePickerDialog = new DatePickerDialog(this, this, ViewModel.EndDate.Year, ViewModel.EndDate.Month - 1, ViewModel.EndDate.Day);
            mDatePickerDialog.Show();
        }

        public void ShowEndTimePicker()
        {
            IsStart = false;
            mTimePickerDialog = new TimePickerDialog(this, this, ViewModel.EndDate.Hour, ViewModel.EndDate.Minute, true);
            mTimePickerDialog.Show();
        }

        public void ShowRepeatPicker()
        {
            
        }

        public void GetEvent()
        {
           // mCalendarService.Events.Get(ViewModel.CalendarId, ViewModel.EventId).FetchAsync(OnEventResponse);
        }

        private void OnEventResponse(LazyResult<Event> _event)
        {
            var realEvent = _event.GetResult();
            ViewModel.EventTitle = realEvent.Summary;
            ViewModel.StartDate = DateTime.Parse(realEvent.Start.DateTime);
            ViewModel.EndDate = DateTime.Parse(realEvent.End.DateTime);
        }

       
       

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            //etHour.Text = hourOfDay.ToString("00") + "h" + minute.ToString("00");
            if (IsStart)
            {
                tvStartTime.Text = hourOfDay.ToString("00") + "h" + minute.ToString("00");
                ViewModel.StartDate = new DateTime(ViewModel.StartDate.Year, ViewModel.StartDate.Month, ViewModel.StartDate.Day,
                    hourOfDay, minute, 0, DateTimeKind.Local);
                
            }
            else
            {
                tvEndTime.Text = hourOfDay.ToString("00") + "h" + minute.ToString("00");
                ViewModel.EndDate = new DateTime(ViewModel.EndDate.Year, ViewModel.EndDate.Month, ViewModel.EndDate.Day,
                    hourOfDay, minute, 0, DateTimeKind.Local);
            }

        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // etDate.Text = (monthOfYear + 1).ToString("00") + "/" + dayOfMonth.ToString("00") + "/" + year;
            if (IsStart)
            {
                tvStartDate.Text = (monthOfYear + 1).ToString("00") + "/" + dayOfMonth.ToString("00") + "/" + year;
                ViewModel.StartDate = new DateTime(year, monthOfYear + 1, dayOfMonth, ViewModel.StartDate.Hour, ViewModel.StartDate.Minute, ViewModel.StartDate.Second, DateTimeKind.Local);


              DateTime PickerStart = new DateTime(year, monthOfYear + 1, dayOfMonth);
              DateTime PickerEnd = new DateTime(ViewModel.EndDate.Year, ViewModel.EndDate.Month, ViewModel.EndDate.Day);

                var Compare = PickerStart.CompareTo(PickerEnd);
                if (Compare == 1)
                {
                    ViewModel.EndDate = new DateTime(year, monthOfYear + 1, dayOfMonth, ViewModel.StartDate.Hour, ViewModel.StartDate.Minute, ViewModel.StartDate.Second, DateTimeKind.Local);
                }


            }
            else
            {
                tvEndDate.Text = (monthOfYear + 1).ToString("00") + "/" + dayOfMonth.ToString("00") + "/" + year;
                ViewModel.EndDate = new DateTime(year, monthOfYear + 1, dayOfMonth, ViewModel.EndDate.Hour, ViewModel.EndDate.Minute, ViewModel.EndDate.Second, DateTimeKind.Local);
            }
        }

        #endregion

        #region Methods

        public void Init()
        {
            tvStartDate = FindViewById<TextView>(Resource.Id.tvStartDate);
            tvStartTime = FindViewById<TextView>(Resource.Id.tvStartTime);
            tvEndDate = FindViewById<TextView>(Resource.Id.tvEndDate);
            tvEndTime = FindViewById<TextView>(Resource.Id.tvEndTime);
            tvAdd = FindViewById<TextView>(Resource.Id.tvAdd);
            tvTitle = FindViewById<TextView>(Resource.Id.tvTitle);



        }


        #endregion


    }
}