using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Fragging.Fragments;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Helpers;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Controls.CalenarDay;
using FlexyPark.UI.Droid.Services;
using Java.Text;
using Java.Util;

namespace FlexyPark.UI.Droid.Activity
{
    public class AddSpotCalendarTabDay : BaseFragment, WeekView.MonthChangeListener, WeekView.EventClickListener, WeekView.EventLongPressListener, IAddSpotCalendarDayView
    {
        #region UI Controls

        #endregion

        #region Variables

        private int Language = 0;
        private static int TYPE_DAY_VIEW = 1;
        private static int TYPE_THREE_DAY_VIEW = 2;
        private static int TYPE_WEEK_VIEW = 3;
        private int mWeekViewType = TYPE_THREE_DAY_VIEW;
        private WeekView mWeekView;
        string[] mArrayColor = new string[] { "#59dbe0", "#f57f68", "#87d288", "#f8b552", "#f57f68", "#87d288", "#59dbe0", "#f57f68", "#87d288", "#f8b552" };



        private AddSpotCalendarViewModel AddSpotCalendarVM;

        #endregion

        #region Constructors

        public AddSpotCalendarTabDay()
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().AddSpotCalendarViewModel;
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.AddSpotCalendarTabDay, container, false);
            AddSpotCalendarVM = ViewModel as AddSpotCalendarViewModel;
            AddSpotCalendarVM.AddSpotCalendarDayView = this;
            string tmp = AddSpotCalendarVM.SharedTextSource.GetText("YesText");

            if (tmp == "Oui")
            {
                Language = 1;
            }
            else
            {
                Language = 0;
            }
            mWeekView = (WeekView)view.FindViewById(Resource.Id.weekView);
            InitWeekView();
            return view;
        }

        #endregion

        #region Implements

        public List<WeekViewEvent> onMonthChange(int newYear, int newMonth)
        {
            List<WeekViewEvent> events = new List<WeekViewEvent>();


            Calendar startTime, endTime;

            for (int i = 0; i < AddSpotCalendarVM.Events.Count; i++)
            {
                if (AddSpotCalendarVM.Events[i].Unavaiability != null)
                {
                    var tmpMonth =
                        AddSpotCalendarVM.Events[i].Unavaiability.StartTimestamp.UnixTimeStampToDateTime().Month - 1;

                    if (tmpMonth == newMonth)
                    {
                        startTime = Calendar.Instance;

                        startTime.Set(Calendar.Month, tmpMonth);
                        var tmpHour =
                            AddSpotCalendarVM.Events[i].Unavaiability.StartTimestamp.UnixTimeStampToDateTime().Hour;
                        startTime.Set(Calendar.HourOfDay, tmpHour);
                        startTime.Set(Calendar.Minute,
                            AddSpotCalendarVM.Events[i].Unavaiability.StartTimestamp.UnixTimeStampToDateTime().Minute);

                        var tmpDay =
                            AddSpotCalendarVM.Events[i].Unavaiability.StartTimestamp.UnixTimeStampToDateTime().Day;
                        startTime.Set(Calendar.DayOfMonth, tmpDay);

                        var tmpYear =
                            AddSpotCalendarVM.Events[i].Unavaiability.StartTimestamp.UnixTimeStampToDateTime().Year;
                        startTime.Set(Calendar.Year, tmpYear);

                        var tmpEndHour =
                            AddSpotCalendarVM.Events[i].Unavaiability.EndTimestamp.UnixTimeStampToDateTime().Hour;

                        endTime = (Calendar)startTime.Clone();
                        endTime.Add(Calendar.Hour, tmpEndHour - tmpHour);
                        endTime.Set(Calendar.Minute,
                            AddSpotCalendarVM.Events[i].Unavaiability.EndTimestamp.UnixTimeStampToDateTime().Minute);
                        endTime.Set(Calendar.DayOfMonth,
                            AddSpotCalendarVM.Events[i].Unavaiability.EndTimestamp.UnixTimeStampToDateTime().Day);
                        var tmpEndMonth =
                            AddSpotCalendarVM.Events[i].Unavaiability.EndTimestamp.UnixTimeStampToDateTime().Month - 1;
                        endTime.Set(Calendar.Month, tmpEndMonth);

                        WeekViewEvent mEvent = new WeekViewEvent(1, AddSpotCalendarVM.Events[i].Unavaiability.Title,
                            startTime, endTime);
                        mEvent.setColor(Color.Gray);
                        events.Add(mEvent);
                    }

                }
                else
                {
                    if (AddSpotCalendarVM.Events[i].Booking != null)
                    {
                        var tmpMonth =
                       AddSpotCalendarVM.Events[i].Booking.StartTimestamp.UnixTimeStampToDateTime().Month - 1;

                        if (tmpMonth == newMonth)
                        {
                            startTime = Calendar.Instance;

                            startTime.Set(Calendar.Month, tmpMonth);
                            var tmpHour =
                                AddSpotCalendarVM.Events[i].Booking.StartTimestamp.UnixTimeStampToDateTime().Hour;
                            startTime.Set(Calendar.HourOfDay, tmpHour);
                            startTime.Set(Calendar.Minute,
                                AddSpotCalendarVM.Events[i].Booking.StartTimestamp.UnixTimeStampToDateTime().Minute);

                            var tmpDay =
                                AddSpotCalendarVM.Events[i].Booking.StartTimestamp.UnixTimeStampToDateTime().Day;
                            startTime.Set(Calendar.DayOfMonth, tmpDay);

                            var tmpYear =
                                AddSpotCalendarVM.Events[i].Booking.StartTimestamp.UnixTimeStampToDateTime().Year;
                            startTime.Set(Calendar.Year, tmpYear);

                            var tmpEndHour =
                                AddSpotCalendarVM.Events[i].Booking.EndTimestamp.UnixTimeStampToDateTime().Hour;

                            endTime = (Calendar)startTime.Clone();
                            endTime.Add(Calendar.Hour, tmpEndHour - tmpHour);
                            endTime.Set(Calendar.Minute,
                                AddSpotCalendarVM.Events[i].Booking.EndTimestamp.UnixTimeStampToDateTime().Minute);
                            endTime.Set(Calendar.DayOfMonth,
                                AddSpotCalendarVM.Events[i].Booking.EndTimestamp.UnixTimeStampToDateTime().Day);
                            var tmpEndMonth =
                                AddSpotCalendarVM.Events[i].Booking.EndTimestamp.UnixTimeStampToDateTime().Month - 1;
                            endTime.Set(Calendar.Month, tmpEndMonth);

                            WeekViewEvent mEvent = new WeekViewEvent(1, AddSpotCalendarVM.Events[i].Booking.PlateNumber,
                                startTime, endTime);
                            mEvent.setColor(Color.Red);
                            events.Add(mEvent);
                        }
                    }
                }

            }

            return events;

        }

        public void onEventClick(WeekViewEvent mEvent, RectF eventRect)
        {
            Toast.MakeText(Activity, "Clicked " + mEvent.getName(), ToastLength.Long).Show();
        }

        public void onEventLongPress(WeekViewEvent mEvent, RectF eventRect)
        {
            Toast.MakeText(Activity, "Long pressed event: " + mEvent.getName(), ToastLength.Long).Show();
        }

        public void GotoToday()
        {
            if (mWeekView != null)
            {
                mWeekView.goToToday();
            }
        }

        public void GotoDateSelected()
        {
            if (AddSpotCalendarVM.DateSelected != null)
            {
                Calendar DateSelected = Calendar.Instance;
                DateSelected.Set(AddSpotCalendarVM.DateSelected.Year, AddSpotCalendarVM.DateSelected.Month - 1, AddSpotCalendarVM.DateSelected.Day, 0, 0);
                mWeekView.goToDate(DateSelected);
            }
        }

        public void UpdateCalendar(bool forceReload = false)
        {
            //InitWeekView();
           // GotoDateSelected();
        }

        #endregion

        #region Methods

        private void setupDateTimeInterpreter(bool shortDate)
        {
            mWeekView.setDateTimeInterpreter(new NewDateTimeInterpreter(shortDate));
        }

        #region NewDateTimeInterpreter

        public class NewDateTimeInterpreter : DateTimeInterpreter
        {
            private bool ShortDate;

            public NewDateTimeInterpreter(bool shortDate)
            {
                ShortDate = shortDate;
            }

            public string interpretDate(Calendar date, int Language)
            {
                SimpleDateFormat weekdayNameFormat = new SimpleDateFormat("EEE", Locale.English);
                string weekday = weekdayNameFormat.Format(date.Time);
                SimpleDateFormat format = new SimpleDateFormat(" M/d", Locale.Default);

                // All android api level do not have a standard way of getting the first letter of
                // the week day name. Hence we get the first char programmatically.
                // Details: http://stackoverflow.com/questions/16959502/get-one-letter-abbreviation-of-week-day-of-a-date-in-java#answer-16959657
                if (ShortDate)
                    // weekday = (weekday.CharAt(0));
                    weekday = (weekday[0]).ToString();

                string SunMon = string.Empty;
                switch (weekday)
                {
                    case "Sun":
                        SunMon = (Language == 1) ? "Dim" : "Sun"; break;
                    case "Mon":
                        SunMon = (Language == 1) ? "Lun" : "Mon"; break;
                    case "Tue":
                        SunMon = (Language == 1) ? "Mar" : "Tue"; break;
                    case "Wed":
                        SunMon = (Language == 1) ? "Mer" : "Wed"; break;
                    case "Thu":
                        SunMon = (Language == 1) ? "Jeu" : "Thu"; break;
                    case "Fri":
                        SunMon = (Language == 1) ? "Ven" : "Fri"; break;
                    case "Sat":
                        SunMon = (Language == 1) ? "Sam" : "Sat"; break;
                    default:
                        SunMon = "The Day";
                        break;
                }


                return SunMon.ToUpper() + format.Format(date.Time);
            }

            public string interpretTime(int hour)
            {
                return hour > 11 ? (hour - 12) + " PM" : (hour == 0 ? "12 AM" : hour + " AM");
            }
        }

        #endregion

        #region InitWeekView

        public void InitWeekView()
        {

            mWeekView.Language = Language;
            mWeekView.setOnEventClickListener(this);
            mWeekView.setMonthChangeListener(this);
            mWeekView.setEventLongPressListener(this);
            setupDateTimeInterpreter(false);
            mWeekView.Init();


            //Day view

            mWeekViewType = TYPE_DAY_VIEW;
            mWeekView.setNumberOfVisibleDays(1);


            // Lets change some dimensions to best fit the view.
            mWeekView.setColumnGap((int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 8, Resources.DisplayMetrics));
            mWeekView.setTextSize((int)TypedValue.ApplyDimension(ComplexUnitType.Sp, 12, Resources.DisplayMetrics));
            mWeekView.setEventTextSize((int)TypedValue.ApplyDimension(ComplexUnitType.Sp, 12, Resources.DisplayMetrics));

           
        }

        #endregion

        #endregion


    }
}