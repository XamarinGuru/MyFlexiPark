using System;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using FlexyPark.Core.Helpers;
using System.Dynamic;
using System.Diagnostics;

namespace FlexyPark.Core.ViewModels
{
    public interface IAddSpotCalendarView
    {
        void SetModeTitle(string title);

        /// <summary>
        /// iOS only.
        /// </summary>
        void ReloadTable();

        void UpdateCalendar(bool forceReload = false);

        //Droid Only
        void GotoToday();
    }

    public interface IAddSpotCalendarTabView
    {
        //Droid only
        void ShowTabDay(DateTime dateTime);

        void GetUnavaiabilities();

    }

    public interface IAddSpotCalendarDayView
    {
        //Droid only
        void GotoToday();

        void GotoDateSelected();

        void UpdateCalendar(bool forceReload = false);
    }

    public class AddSpotCalendarViewModel : BaseViewModel
    {
        private DateTime currentStartDateTime, currentEndDateTime;

        #region Constructors

        public AddSpotCalendarViewModel(ICacheService cacheService, IApiService apiService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init(string parkingId)
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    Events.Add(new EventItemViewModel(this)
            //        {
            //            Summary = "EventTitle",
            //            StartDate = DateTime.UtcNow.ToLocalTime(),
            //            EndDate = DateTime.UtcNow.ToLocalTime().AddHours(2),
            //            IsEditMode = false,
            //            IsSelected = false
            //        });   
            //}

            if (string.IsNullOrEmpty(parkingId))
                ParkingId = Mvx.Resolve<ICacheService>().ParkingId;
            else
                ParkingId = parkingId;

            await Task.Delay(100);

            //GetUnavaiabilities(DateTime.UtcNow);
        }

        #endregion

        #region View

        public IAddSpotCalendarView View { get; set; }

        public IAddSpotCalendarTabView AddSpotCalendarTabView { get; set; }

        public IAddSpotCalendarDayView AddSpotCalendarDayView { get; set; }

        #endregion

        #region Properties

        #region DateSelected

        private DateTime mDateSelected = DateTime.Today;

        public DateTime DateSelected
        {
            get { return mDateSelected; }
            set
            {
                mDateSelected = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region NeedToClose

        private bool mNeedToClose = false;

        public bool NeedToClose
        {
            get
            {
                return mNeedToClose;
            }
            set
            {
                mNeedToClose = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsEditMode

        private bool mIsEditMode = false;

        public bool IsEditMode
        {
            get
            {
                return mIsEditMode;
            }
            set
            {
                mIsEditMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region CalendarId

        private string mCalendarId = string.Empty;

        public string CalendarId
        {
            get
            {
                return mCalendarId;
            }
            set
            {
                mCalendarId = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region SaveEventsUnsort

        private ObservableCollection<EventItemViewModel> SaveEventsUnsort = new ObservableCollection<EventItemViewModel>();

        #endregion

        #region Events

        private ObservableCollection<EventItemViewModel> mEvents = new ObservableCollection<EventItemViewModel>();

        public ObservableCollection<EventItemViewModel> Events
        {
            get
            {
                return mEvents;
            }
            set
            {
                mEvents = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EventFilterByDay

        private ObservableCollection<EventItemViewModel> mEventFilterByDay = new ObservableCollection<EventItemViewModel>();

        public ObservableCollection<EventItemViewModel> EventFilterByDay
        {
            get { return mEventFilterByDay; }
            set
            {
                mEventFilterByDay = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region SelectedEvents

        private ObservableCollection<EventItemViewModel> mSelectedEvents = new ObservableCollection<EventItemViewModel>();

        public ObservableCollection<EventItemViewModel> SelectedEvents
        {
            get
            {
                return mSelectedEvents;
            }
            set
            {
                mSelectedEvents = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region SelectedDate

        private DateTime mSelectedDate = DateTime.UtcNow.ToLocalTime();

        public DateTime SelectedDate
        {
            get
            {
                return mSelectedDate;
            }
            set
            {
                mSelectedDate = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PagerSelected

        // Droid Only

        private int mPagerSelected = 100;

        public int PagerSelected
        {
            get
            {
                return mPagerSelected;
            }

            set
            {
                mPagerSelected = value;
                RaisePropertyChanged();
            }

        }

        #endregion

        #region ParkingId

        private string mParkingId = string.Empty;

        public string ParkingId
        {
            get
            {
                return mParkingId;
            }
            set
            {
                mParkingId = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region SelectedEvent

        public EventItemViewModel SelectedEvent { get; set; }

        #endregion

        #endregion

        #region Commands

        #region SwitchModeCommand

        private MvxCommand mSwitchModeCommand = null;

        public MvxCommand SwitchModeCommand
        {
            get
            {
                if (mSwitchModeCommand == null)
                {
                    mSwitchModeCommand = new MvxCommand(this.SwitchMode);
                }
                return mSwitchModeCommand;
            }
        }

        public void SwitchMode()
        {
            IsEditMode = !IsEditMode;
            foreach (var _event in Events)
            {
                _event.IsEditMode = IsEditMode;
            }

            if (View != null)
                View.SetModeTitle(IsEditMode ? "Cancel" : "Edit");
        }

        #endregion

        #region DoneCommand

        private MvxCommand mDoneCommand = null;

        public MvxCommand DoneCommand
        {
            get
            {
                if (mDoneCommand == null)
                {
                    mDoneCommand = new MvxCommand(this.Done);
                }
                return mDoneCommand;
            }
        }

        private void Done()
        {
            /*if (SelectedEvents.Count > 0)
            {
                mCacheService.NextStatus = AddSpotStatus.Complete;
                Close(this);
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Please select at lease one unavailability !"));
            }*/

            mCacheService.NextStatus = AddSpotStatus.Complete;
            Close(this);
        }

        #endregion

        #region AddNewEventCommand

        private MvxCommand mAddNewEventCommand = null;

        public MvxCommand AddNewEventCommand
        {
            get
            {
                if (mAddNewEventCommand == null)
                    mAddNewEventCommand = new MvxCommand(this.AddNewEvent);
                return mAddNewEventCommand;
            }
        }

        private void AddNewEvent()
        {
            ShowViewModel<AddEventViewModel>(new { isEditMode = false });

            /*ShowViewModelWithCallback<AddEventViewModel, bool>(
                new Dictionary<string, string>()
                {
                    {"isEditMode" , "false"},
                    {"eventId", string.Empty},
                    {"calendarId", CalendarId},
                },
                mess => {
                GetEvent();
            } );*/
        }

        #endregion

        #region EventItemClickedCommand

        private MvxCommand<EventItemViewModel> mEventItemClickedCommand = null;

        public MvxCommand<EventItemViewModel> EventItemClickedCommand
        {
            get
            {
                if (mEventItemClickedCommand == null)
                {
                    mEventItemClickedCommand = new MvxCommand<EventItemViewModel>(this.EventItemClicked);
                }
                return mEventItemClickedCommand;
            }
        }

        private void EventItemClicked(EventItemViewModel eventVM)
        {
            if (eventVM.Unavaiability == null) {
                Debug.WriteLine ("Receive click on Booking ???");
                SelectedEvent = eventVM;
                ShowViewModel<AddEventViewModel> (new {
                    unavailabilityId = string.Empty,
                    parkingId = ParkingId,
                    isEditMode = true,
                    title = eventVM.Booking.PlateNumber,
                    startTime = eventVM.Booking.StartTimestamp,
                    endTime = eventVM.Booking.EndTimestamp,
                    repeat = (eventVM.Unavaiability != null && eventVM.Unavaiability.Periodicity != null && !string.IsNullOrEmpty (eventVM.Unavaiability.Periodicity.Repeat)) ? eventVM.Unavaiability.Periodicity.Repeat : string.Empty,
                    time = (eventVM.Unavaiability != null &&  eventVM.Unavaiability.Periodicity != null && !string.IsNullOrEmpty (eventVM.Unavaiability.Periodicity.OccurrencesAmount)) ? eventVM.Unavaiability.Periodicity.OccurrencesAmount : string.Empty
                });

                //mCacheService.CurrentReservation = eventVM.Booking;
                //ShowViewModel<ParkingReservedViewModel> (new { status = ParkingStatus.Reserved, isReadOnly = true });
            } else {
                var ParentEvent = SaveEventsUnsort.FirstOrDefault (x => x.Unavaiability.UnavailabilityId == eventVM.Unavaiability.UnavailabilityId);
                long block = ParentEvent.Unavaiability.EndTimestamp - ParentEvent.Unavaiability.StartTimestamp;
                long starTimestamp = ParentEvent.Unavaiability.StartTimestamp;
                long endTimestamp = ParentEvent.Unavaiability.EndTimestamp;

                var occurences = ParentEvent.Unavaiability.Periodicity.Occurrences.FirstOrDefault (x => x.StartTimestamp == eventVM.Unavaiability.StartTimestamp);
                if (occurences != null) {
                    starTimestamp = occurences.StartTimestamp;
                    endTimestamp = starTimestamp + block;
                }
                if (eventVM.Unavaiability != null && ParentEvent != null) {
                    SelectedEvent = eventVM;
                    ShowViewModel<AddEventViewModel> (new {
                        unavailabilityId = eventVM.Unavaiability != null ? eventVM.Unavaiability.UnavailabilityId : string.Empty,
                        parkingId = ParkingId,
                        isEditMode = true,
                        title = eventVM.Unavaiability.Title,
                        startTime = starTimestamp,
                        endTime = endTimestamp,
                        repeat = (eventVM.Unavaiability.Periodicity != null && !string.IsNullOrEmpty (eventVM.Unavaiability.Periodicity.Repeat)) ? eventVM.Unavaiability.Periodicity.Repeat : string.Empty,
                        time = (eventVM.Unavaiability.Periodicity != null && !string.IsNullOrEmpty(eventVM.Unavaiability.Periodicity.OccurrencesAmount)) ? eventVM.Unavaiability.Periodicity.OccurrencesAmount : string.Empty
				    });
                }
            }
        }

        #endregion

        #region DeleteEventCommand

        private MvxCommand<EventItemViewModel> mDeleteEventCommand = null;

        public MvxCommand<EventItemViewModel> DeleteEventCommand
        {
            get
            {
                if (mDeleteEventCommand == null)
                {
                    mDeleteEventCommand = new MvxCommand<EventItemViewModel>(this.DeleteEvent);
                }
                return mDeleteEventCommand;
            }
        }

        private void DeleteEvent(EventItemViewModel eventVM)
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("DeleteEventText"), SharedTextSource.GetText("AreYouSureDeleteEventText"), SharedTextSource.GetText("NoText"), null, new string[] { SharedTextSource.GetText("DeleteEventOnlyText"), SharedTextSource.GetText("DeleteFutureEventsText") },
                    async () =>
                    {
                        if (BaseView != null && BaseView.CheckInternetConnection())
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                            var result = await mApiService.DeleteUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, eventVM.Unavaiability.UnavailabilityId);
                            if (result != null)
                            {
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}: {1}", result.Result, result.ErrorCode)));
                                if (result)
                                {
                                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Success"));
                                    if (Events.Contains(eventVM))
                                        Events.Remove(eventVM);
                                    if (View != null)
                                        View.UpdateCalendar(true);



                                }
                                else
                                {
                                    Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", "", ""), "Ok", null));
                                }
                            }
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                        }
                        else
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                        }
                    },
                    async () =>
                    {
                        if (BaseView != null && BaseView.CheckInternetConnection())
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                            var result = await mApiService.DeleteUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, eventVM.Unavaiability.UnavailabilityId);

                            if (result != null)
                            {
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}: {1}", result.Result, result.ErrorCode)));
                                if (result)
                                {
                                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Success"));
                                    if (View != null)
                                        View.UpdateCalendar();


                                }
                                else
                                {
                                    Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", "", ""), "Ok", null));
                                }
                            }
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));

                            // Android only: When dismiss delete dialog, Android cannot call resume to call GetUnavaiabilities so we call manual
                            if (AddSpotCalendarTabView != null)
                            {
                                AddSpotCalendarTabView.GetUnavaiabilities();
                            }
                            ///////////////////////////////
                        }
                        else
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                        }
                    }));


        }

        #endregion

        #region TodayCommand

        private MvxCommand mTodayCommand;

        public MvxCommand TodayCommand
        {
            get
            {
                if (mTodayCommand == null)
                {
                    mTodayCommand = new MvxCommand(this.Today);
                }
                return mTodayCommand;

            }
        }

        private void Today()
        {
            if (View != null)
            {
                View.GotoToday();
            }
            if (AddSpotCalendarDayView != null)
            {
                AddSpotCalendarDayView.GotoToday();
            }
        }

        #endregion

        #endregion

        #region Methods

        #region GetUnavaiabilities

        public async void GetUnavaiabilities(DateTime startDateTime)
        {
            if (currentStartDateTime != null && currentStartDateTime.Month == startDateTime.Month && !mCacheService.NeedReloadEvent)
                return;
            
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                Events.Clear();

                if (mCacheService.NeedReloadEvent)
                    mCacheService.NeedReloadEvent = false;

                currentStartDateTime = startDateTime;
                currentEndDateTime = startDateTime.AddDays (30);

                var results = await mApiService.OwnerUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, Helpers.TimestampHelpers.DateTimeToTimeStamp(currentStartDateTime), Helpers.TimestampHelpers.DateTimeToTimeStamp(currentEndDateTime), "startTimestamp");
                if (results != null && results.Response.Count != 0)
                {
                    foreach (var item in results.Response)
                    {
                        Events.Add(new EventItemViewModel(this)
                            {
                                Unavaiability = item,
                                IsEditMode = false,
                                IsSelected = false,
                            });
                    }
                }

                var results2 = await mApiService.OwnerBookings(mCacheService.CurrentUser.UserId, ParkingId, Helpers.TimestampHelpers.DateTimeToTimeStamp(currentStartDateTime), Helpers.TimestampHelpers.DateTimeToTimeStamp(currentEndDateTime), "startTimestamp");
                if (results2 != null && results2.Response.Count != 0)
                {
                    foreach (var item in results2.Response)
                    {
                        Events.Add(new EventItemViewModel(this)
                            {
                                Booking = item,
                                IsEditMode = false,
                                IsSelected = false,
                            });
                    }
                }


                if (Events != null && Events.Count > 0)
                {
                    SaveEventsUnsort = new ObservableCollection<EventItemViewModel>(Events);
                    NewSortUnavaiabilities();
                }
                else
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "No event!"));
                
                if (View != null)
                    View.UpdateCalendar(true);
                
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion


        #region SortUnavaiabilities

        private void SortUnavaiabilities()
        {
            List<EventItemViewModel> removedEvents = new List<EventItemViewModel>();
            List<EventItemViewModel> addedEvents = new List<EventItemViewModel>();

            //checking Periodicity
            foreach (var unavaiability in Events)
            {
                if (unavaiability.Unavaiability == null)
                    continue;

                if (unavaiability.Unavaiability.Periodicity == null || string.IsNullOrEmpty(unavaiability.Unavaiability.Periodicity.Repeat) || unavaiability.Unavaiability.Periodicity.Repeat.Equals("none"))
                    continue;

                if (string.IsNullOrEmpty(unavaiability.Unavaiability.Periodicity.OccurrencesAmount))
                    continue;

                switch (unavaiability.Unavaiability.Periodicity.Repeat)
                {
                    case "everyDay":
                        for (int i = 0; i < Int32.Parse(unavaiability.Unavaiability.Periodicity.OccurrencesAmount); i++)
                        {
                            addedEvents.Add(new EventItemViewModel(this)
                                {
                                    Unavaiability = new OwnerUnavailability()
                                    {
                                        UnavailabilityId = unavaiability.Unavaiability.UnavailabilityId,
                                        Title = unavaiability.Unavaiability.Title,
                                        StartTimestamp = unavaiability.Unavaiability.StartTimestamp + 3600 * 24 * (i + 1),
                                        EndTimestamp = unavaiability.Unavaiability.EndTimestamp + 3600 * 24 * (i + 1),
                                        NumberOfSpots = unavaiability.Unavaiability.NumberOfSpots,
                                        Periodicity = unavaiability.Unavaiability.Periodicity
                                    },
                                    IsEditMode = false,
                                    IsSelected = false,
                                });
                        }

                        if (unavaiability.Unavaiability.Periodicity.Exceptions != null && unavaiability.Unavaiability.Periodicity.Exceptions.Count > 0)
                        {
                            foreach (var exception in unavaiability.Unavaiability.Periodicity.Exceptions)
                            {
                                var needToRemove = addedEvents.FirstOrDefault(x => x.Unavaiability.StartTimestamp == exception.StartTimestamp);
                                if (needToRemove != null)
                                    addedEvents.Remove(needToRemove);
                            }
                        }

                        break;
                    case "everyWeek":
                        for (int i = 0; i < Int32.Parse(unavaiability.Unavaiability.Periodicity.OccurrencesAmount); i++)
                        {
                            addedEvents.Add(new EventItemViewModel(this)
                                {
                                    Unavaiability = new OwnerUnavailability()
                                    {
                                        UnavailabilityId = unavaiability.Unavaiability.UnavailabilityId,
                                        Title = unavaiability.Unavaiability.Title,
                                        StartTimestamp = unavaiability.Unavaiability.StartTimestamp + 3600 * 24 * 7 * (i + 1),
                                        EndTimestamp = unavaiability.Unavaiability.EndTimestamp + 3600 * 24 * 7 * (i + 1),
                                        NumberOfSpots = unavaiability.Unavaiability.NumberOfSpots,
                                        Periodicity = unavaiability.Unavaiability.Periodicity
                                    },
                                    IsEditMode = false,
                                    IsSelected = false,
                                });
                        }

                        if (unavaiability.Unavaiability.Periodicity.Exceptions != null && unavaiability.Unavaiability.Periodicity.Exceptions.Count > 0)
                        {
                            foreach (var exception in unavaiability.Unavaiability.Periodicity.Exceptions)
                            {
                                var needToRemove = addedEvents.FirstOrDefault(x => x.Unavaiability.StartTimestamp == exception.StartTimestamp);
                                if (needToRemove != null)
                                    addedEvents.Remove(needToRemove);
                            }
                        }
                        break;
                    case "everyMonth":

                        break;
                }
            }

            //add new events
            if (addedEvents.Count > 0)
            {
                foreach (var item in addedEvents)
                {
                    Events.Add(item);
                }
            }
            addedEvents.Clear();


            foreach (var unavaiability in Events)
            {
                //when there are events that go throught more than 1 day
                if (unavaiability.StartDate.Date != unavaiability.EndDate.Date)
                {
                    removedEvents.Add(unavaiability);

                    int countOldyear, countNewyear = 0;

                    if (unavaiability.EndDate.Year > unavaiability.StartDate.Year)
                    {
                        countNewyear = unavaiability.EndDate.DayOfYear;
                        DateTime tmp = new DateTime(unavaiability.StartDate.Year, 12, 31);
                        countOldyear = Math.Abs(tmp.DayOfYear - unavaiability.StartDate.DayOfYear);
                    }
                    else
                    {
                        countOldyear = unavaiability.EndDate.DayOfYear - unavaiability.StartDate.DayOfYear;
                    }

                    //separate 1 event into 'count' new events
                    DateTime startDate = unavaiability.StartDate;
                    for (int i = 0; i < countOldyear + 1; i++)
                    {
                        DateTime endDatetmp = new DateTime();
                        if (countNewyear > 0)
                            endDatetmp = new DateTime(unavaiability.StartDate.Year, 12, 31);
                        else
                            endDatetmp = unavaiability.EndDate;
                        if (startDate.DayOfYear <= endDatetmp.DayOfYear)
                        {
                            //get the end of the StartDate ( 12 am )
                            var nextDay = startDate.Day + 1;
                            var nextMonth = startDate.Month;
                            if (nextDay > DateTime.DaysInMonth(startDate.Year, startDate.Month))
                            {
                                nextDay = 1;
                                if (nextMonth < 12)
                                    nextMonth++;
                                else
                                    nextMonth = 1;
                            }

                            var endOfStartDate = new DateTime(startDate.Year, nextMonth, nextDay, 00, 00, 00, DateTimeKind.Local);

                            if (endOfStartDate.Ticks > endDatetmp.Ticks)
                                endOfStartDate = unavaiability.EndDate;

                            if (unavaiability.Unavaiability != null)
                            {
                                addedEvents.Add(new EventItemViewModel(this)
                                    {
                                        Unavaiability = new OwnerUnavailability()
                                        {
                                            UnavailabilityId = unavaiability.Unavaiability.UnavailabilityId,
                                            Title = unavaiability.Unavaiability.Title,
                                            StartTimestamp = startDate.DateTimeToTimeStamp(),
                                            EndTimestamp = endOfStartDate.DateTimeToTimeStamp(),
                                            NumberOfSpots = unavaiability.Unavaiability.NumberOfSpots,
                                            Periodicity = unavaiability.Unavaiability.Periodicity
                                        },
                                        IsEditMode = false,
                                        IsSelected = false,
                                    });
                            }


                            startDate = endOfStartDate;
                        }
                    }

                    startDate = new DateTime(unavaiability.EndDate.Year, 1, 1);
                    for (int i = 0; i < countNewyear; i++)
                    {
                        if (startDate.DayOfYear <= unavaiability.EndDate.Date.DayOfYear)
                        {
                            //get the end of the StartDate ( 12 am )
                            var nextDay = startDate.Day + 1;
                            var nextMonth = startDate.Month;
                            if (nextDay > DateTime.DaysInMonth(startDate.Year, startDate.Month))
                            {
                                nextDay = 1;
                                if (nextMonth < 12)
                                    nextMonth++;
                                else
                                    nextMonth = 1;
                            }

                            var endOfStartDate = new DateTime(startDate.Year, nextMonth, nextDay, 00, 00, 00, DateTimeKind.Local);

                            if (endOfStartDate.Ticks > unavaiability.EndDate.Ticks)
                                endOfStartDate = unavaiability.EndDate;

                            if (unavaiability.Unavaiability != null)
                            {
                                addedEvents.Add(new EventItemViewModel(this)
                                    {
                                        Unavaiability = new OwnerUnavailability()
                                        {
                                            UnavailabilityId = unavaiability.Unavaiability.UnavailabilityId,
                                            Title = unavaiability.Unavaiability.Title,
                                            StartTimestamp = startDate.DateTimeToTimeStamp(),
                                            EndTimestamp = endOfStartDate.DateTimeToTimeStamp(),
                                            NumberOfSpots = unavaiability.Unavaiability.NumberOfSpots,
                                            Periodicity = unavaiability.Unavaiability.Periodicity
                                        },
                                        IsEditMode = false,
                                        IsSelected = false,
                                    });
                            }


                            startDate = endOfStartDate;
                        }
                    }



                }
            }


            //remove all old events
            if (removedEvents.Count > 0)
            {
                foreach (var item in removedEvents)
                {
                    Events.Remove(item);
                }
            }

            //add new events
            if (addedEvents.Count > 0)
            {
                foreach (var item in addedEvents)
                {
                    Events.Add(item);
                }
            }

            //sort the events again
            var sortedList = Events.OrderBy(x => x.StartDate);
            Events = new ObservableCollection<EventItemViewModel>(sortedList);
        }

        #endregion

        #region NewSortUnavaiabilities

        private void NewSortUnavaiabilities()
        {
            List<EventItemViewModel> removedEvents = new List<EventItemViewModel>();
            List<EventItemViewModel> addedEvents = new List<EventItemViewModel>();

            //checking Periodicity
            foreach (var unavaiability in Events)
            {
                if (unavaiability.Unavaiability == null && unavaiability.Booking == null)
                    continue;

                if (unavaiability.Booking != null) {
                    
                    addedEvents.Add (new EventItemViewModel (this) {
                        Booking = unavaiability.Booking,
                        IsEditMode = false,
                        IsSelected = false,
                    });

                } else {
                    if (unavaiability.Unavaiability.Periodicity == null || string.IsNullOrEmpty (unavaiability.Unavaiability.Periodicity.Repeat) || unavaiability.Unavaiability.Periodicity.Repeat.Equals ("none"))
                        continue;

                    if (string.IsNullOrEmpty (unavaiability.Unavaiability.Periodicity.OccurrencesAmount))
                        continue;

                    if (unavaiability.Unavaiability.Periodicity.Occurrences.Count == 0)
                        continue;
                    else {
                        long block = unavaiability.Unavaiability.EndTimestamp - unavaiability.Unavaiability.StartTimestamp;
                        foreach (var time in unavaiability.Unavaiability.Periodicity.Occurrences) {
                            addedEvents.Add (new EventItemViewModel (this) {
                                Unavaiability = new OwnerUnavailability () {
                                    UnavailabilityId = unavaiability.Unavaiability.UnavailabilityId,
                                    Title = unavaiability.Unavaiability.Title,
                                    StartTimestamp = time.StartTimestamp,
                                    EndTimestamp = time.StartTimestamp + block,
                                    NumberOfSpots = unavaiability.Unavaiability.NumberOfSpots,
                                    Periodicity = unavaiability.Unavaiability.Periodicity
                                },
                                IsEditMode = false,
                                IsSelected = false,
                            });
                        }
                    }
                }
            }
            Events.Clear();
            //add new events
            if (addedEvents.Count > 0)
            {
                foreach (var item in addedEvents)
                {
                    Events.Add(item);
                }
            }
            addedEvents.Clear();


            foreach (var unavaiability in Events)
            {
                //when there are events that go throught more than 1 day
                if (unavaiability.StartDate.Date != unavaiability.EndDate.Date)
                {
                    removedEvents.Add(unavaiability);

                    int countOldyear, countNewyear = 0;

                    if (unavaiability.EndDate.Year > unavaiability.StartDate.Year)
                    {
                        countNewyear = unavaiability.EndDate.DayOfYear;
                        DateTime tmp = new DateTime(unavaiability.StartDate.Year, 12, 31);
                        countOldyear = Math.Abs(tmp.DayOfYear - unavaiability.StartDate.DayOfYear);
                    }
                    else
                    {
                        countOldyear = unavaiability.EndDate.DayOfYear - unavaiability.StartDate.DayOfYear;
                    }

                    //separate 1 event into 'count' new events
                    DateTime startDate = unavaiability.StartDate;
                    for (int i = 0; i < countOldyear + 1; i++)
                    {
                        DateTime endDatetmp = new DateTime();
                        if (countNewyear > 0)
                            endDatetmp = new DateTime(unavaiability.StartDate.Year, 12, 31);
                        else
                            endDatetmp = unavaiability.EndDate;
                        if (startDate.DayOfYear <= endDatetmp.DayOfYear)
                        {
                            //get the end of the StartDate ( 12 am )
                            var nextDay = startDate.Day + 1;
                            var nextMonth = startDate.Month;
                            if (nextDay > DateTime.DaysInMonth(startDate.Year, startDate.Month))
                            {
                                nextDay = 1;
                                if (nextMonth < 12)
                                    nextMonth++;
                                else
                                    nextMonth = 1;
                            }

                            var endOfStartDate = new DateTime(startDate.Year, nextMonth, nextDay, 00, 00, 00, DateTimeKind.Local);

                            if (endOfStartDate.Ticks > endDatetmp.Ticks)
                                endOfStartDate = unavaiability.EndDate;

                            if (unavaiability.Unavaiability != null)
                            {
                                addedEvents.Add(new EventItemViewModel(this)
                                    {
                                        Unavaiability = new OwnerUnavailability()
                                        {
                                            UnavailabilityId = unavaiability.Unavaiability.UnavailabilityId,
                                            Title = unavaiability.Unavaiability.Title,
                                            StartTimestamp = startDate.DateTimeToTimeStamp(),
                                            EndTimestamp = endOfStartDate.DateTimeToTimeStamp(),
                                            NumberOfSpots = unavaiability.Unavaiability.NumberOfSpots,
                                            Periodicity = unavaiability.Unavaiability.Periodicity
                                        },
                                        IsEditMode = false,
                                        IsSelected = false,
                                    });
                            }


                            startDate = endOfStartDate;
                        }
                    }

                    startDate = new DateTime(unavaiability.EndDate.Year, 1, 1);
                    for (int i = 0; i < countNewyear; i++)
                    {
                        if (startDate.DayOfYear <= unavaiability.EndDate.Date.DayOfYear)
                        {
                            //get the end of the StartDate ( 12 am )
                            var nextDay = startDate.Day + 1;
                            var nextMonth = startDate.Month;
                            if (nextDay > DateTime.DaysInMonth(startDate.Year, startDate.Month))
                            {
                                nextDay = 1;
                                if (nextMonth < 12)
                                    nextMonth++;
                                else
                                    nextMonth = 1;
                            }

                            var endOfStartDate = new DateTime(startDate.Year, nextMonth, nextDay, 00, 00, 00, DateTimeKind.Local);

                            if (endOfStartDate.Ticks > unavaiability.EndDate.Ticks)
                                endOfStartDate = unavaiability.EndDate;

                            if (unavaiability.Unavaiability != null)
                            {
                                addedEvents.Add(new EventItemViewModel(this)
                                    {
                                        Unavaiability = new OwnerUnavailability()
                                        {
                                            UnavailabilityId = unavaiability.Unavaiability.UnavailabilityId,
                                            Title = unavaiability.Unavaiability.Title,
                                            StartTimestamp = startDate.DateTimeToTimeStamp(),
                                            EndTimestamp = endOfStartDate.DateTimeToTimeStamp(),
                                            NumberOfSpots = unavaiability.Unavaiability.NumberOfSpots,
                                            Periodicity = unavaiability.Unavaiability.Periodicity
                                        },
                                        IsEditMode = false,
                                        IsSelected = false,
                                    });
                            }


                            startDate = endOfStartDate;
                        }
                    }



                }
            }


            //remove all old events
            if (removedEvents.Count > 0)
            {
                foreach (var item in removedEvents)
                {
                    Events.Remove(item);
                }
            }

            //add new events
            if (addedEvents.Count > 0)
            {
                foreach (var item in addedEvents)
                {
                    Events.Add(item);
                }
            }

            //sort the events again
            var sortedList = Events.OrderBy(x => x.StartDate);
            Events = new ObservableCollection<EventItemViewModel>(sortedList);
        }

        #endregion


        public void CloseViewModel()
        {
            Close(this);
        }

        //        public async void DeleteUnavaiability()
        //        {
        //            if (BaseView != null && BaseView.CheckInternetConnection())
        //            {
        //                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
        //                var result = await mApiService.DeleteUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, );
        //
        //                if (result != null)
        //                {
        //                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
        //                    //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}: {1}", result.Result, result.ErrorCode)));
        //                    if (result)
        //                    {
        //                        Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result));
        //                    }
        //                    else
        //                    {
        //                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", "", ""), "Ok", null));
        //                    }
        //                }
        //                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
        //            }
        //            else
        //            {
        //                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
        //            }
        //        }

        public void ShowTabDay(DateTime mDateTime)
        {
            if (AddSpotCalendarTabView != null)
            {
                DateSelected = mDateTime;
                AddSpotCalendarTabView.ShowTabDay(mDateTime);
            }
        }

        #endregion
    }
}

