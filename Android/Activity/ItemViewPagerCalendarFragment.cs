using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.ViewModels;
using FlexyPark.UI.Droid.Services;
using Java.Util;
using MonoDroid.TimesSquare;
using Calendar = System.Globalization.Calendar;
using GregorianCalendar = System.Globalization.GregorianCalendar;

namespace FlexyPark.UI.Droid.Activity
{
    public class ItemViewPagerCalendarFragment : MvxFragment
    {

        #region Variables

        private int month = 0;
        private MvxDialogFragment dialog;
        List<DateTime> ListDateEvent = new List<DateTime>();
        public CalendarPickerView calendar;



        public AddSpotCalendarViewModel AddSpotCalendarViewModelVM;


        public ItemViewPagerCalendarFragment(int position)
        {
            ViewModel = Mvx.Resolve<IFixMvvmCross>().AddSpotCalendarViewModel;
            month = position;
        }

        #endregion

        #region Overrides

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            this.EnsureBindingContextIsSet(savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.ItemViewPagerCalendar, container, false);
            AddSpotCalendarViewModelVM = ViewModel as AddSpotCalendarViewModel;

            calendar = view.FindViewById<CalendarPickerView>(Resource.Id.calendaritem);

            if (AddSpotCalendarViewModelVM.Events != null && AddSpotCalendarViewModelVM.Events.Count != 0)
            {
                ListDateEvent.Clear();
                for (int i = 0; i < AddSpotCalendarViewModelVM.Events.Count; i++)
                {

                    if (AddSpotCalendarViewModelVM.Events[i].StartDate.Month == DateTime.UtcNow.AddMonths(month).Month &&
                        AddSpotCalendarViewModelVM.Events[i].StartDate.Year == DateTime.UtcNow.AddMonths(month).Year)
                    {
                        if (ListDateEvent.Count != 0)
                        {
                            var IsExits = false;
                            for (int j = 0; j < ListDateEvent.Count; j++)
                            {
                                if (ListDateEvent[j].Day == AddSpotCalendarViewModelVM.Events[i].StartDate.Day)
                                {
                                    IsExits = true;
                                }

                            }
                            if (!IsExits)
                            {
                                ListDateEvent.Add(AddSpotCalendarViewModelVM.Events[i].StartDate);
                            }
                        }
                        else
                        {
                            ListDateEvent.Add(AddSpotCalendarViewModelVM.Events[i].StartDate);
                        }
                    }

                }

            }
            else
            {
                if (ListDateEvent.Count != 0)
                {
                    ListDateEvent.Clear();
                }

            }

            int NumberLanguage = 0;
            var LanguageText = AddSpotCalendarViewModelVM.SharedTextSource.GetText("YesText");
            switch (LanguageText)
            {
                case "Oui":
                    NumberLanguage = 1; break;
                default:
                    NumberLanguage = 0; break;

            }

            calendar.Language = NumberLanguage;

            //for (int j = 1; j <= DateTime.DaysInMonth(2015, 11); j++)
            //    {
            //        ListDateEvent.Add(new DateTime(2015, 11, j));
            //    }

            if (AddSpotCalendarViewModelVM.DateSelected != null)
            {
                calendar.Init(DateTime.UtcNow.AddMonths(month).AddDays(-(DateTime.UtcNow.Day - 1)), DateTime.UtcNow.AddMonths(month + 1).AddDays(-(DateTime.UtcNow.Day + 1)))
                .InMode(CalendarPickerView.SelectionMode.Single)
                .WithSelectedDate(AddSpotCalendarViewModelVM.DateSelected)
                .WithHighlightedDates(ListDateEvent);

                if (ListDateEvent.Count != 0)
                {
                    foreach (var dateTime in ListDateEvent)
                    {
                        Console.WriteLine(dateTime.ToString() + "\n");
                    }
                }

            }
            else
            {
                calendar.Init(DateTime.UtcNow.AddMonths(month).AddDays(-(DateTime.UtcNow.Day - 1)), DateTime.UtcNow.AddMonths(month + 1).AddDays(-(DateTime.UtcNow.Day + 1)))
                .InMode(CalendarPickerView.SelectionMode.Single)
                .WithHighlightedDates(ListDateEvent);
            }



            calendar.OnDateSelected +=
                (s, e) =>
                {
                    UpdateEventFilterByDay(e.SelectedDate);
                };

            UpdateEventFilterByDay(AddSpotCalendarViewModelVM.SelectedDate);

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        #endregion

        #region Method

        #region UpdateEventFilterByDay

        public void UpdateEventFilterByDay(DateTime selectedDate)
        {
            AddSpotCalendarViewModelVM.DateSelected = selectedDate;

            if (AddSpotCalendarViewModelVM.EventFilterByDay != null &&
                AddSpotCalendarViewModelVM.EventFilterByDay.Count != 0)
            {
                AddSpotCalendarViewModelVM.EventFilterByDay.Clear();
            }

            if (AddSpotCalendarViewModelVM.Events != null && AddSpotCalendarViewModelVM.Events.Count != 0)
            {
                for (int i = 0; i < AddSpotCalendarViewModelVM.Events.Count; i++)
                {
                    if (AddSpotCalendarViewModelVM.Events[i].StartDate.DayOfYear == selectedDate.DayOfYear)
                    {
                        AddSpotCalendarViewModelVM.EventFilterByDay.Add(AddSpotCalendarViewModelVM.Events[i]);
                    }
                }
            }
        }


        #endregion

        #endregion
    }
}