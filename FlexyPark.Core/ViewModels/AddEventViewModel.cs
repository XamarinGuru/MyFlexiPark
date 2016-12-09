using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using FlexyPark.Core.Messengers;
using System.Collections.Generic;
using FlexyPark.Core.Helpers;
using System.Linq;


namespace FlexyPark.Core.ViewModels
{
    public interface IAddEventView
    {
        /// <summary>
        /// Shows the date picker of start date.
        /// </summary>
        void ShowStartDatePicker();

        /// <summary>
        /// Shows the time picker  of start date.
        /// </summary>
        void ShowStartTimePicker();

        /// <summary>
        /// Shows the date picker of end date.
        /// </summary>
        void ShowEndDatePicker();

        /// <summary>
        /// Shows the time picker  of end date.
        /// </summary>
        void ShowEndTimePicker();

        /// <summary>
        /// Shows the repeat picker.
        /// </summary>
        void ShowRepeatPicker();

        void GetEvent();
    }

    public class AddEventViewModel : BaseViewModel
    {
        private readonly ICacheService mCacheService;
        private readonly IApiService mApiService;

        private string mOldRepeat;

        #region Constructors

        public AddEventViewModel(ICacheService cacheService, IApiService apiService)
            : base(apiService, cacheService)
        {
            this.mCacheService = cacheService;
            this.mApiService = apiService;
        }

        #endregion

        #region Init

        public void Init(string unavailabilityId, string parkingId, bool isEditMode, string title, long startTime, long endTime, string repeat, string time)
        {
            IsEditMode = isEditMode;
            //Title = isEditMode ? "Edit Event" : "Add Event";

            ButtonTitle = isEditMode ? "Edit" : "Add";

            IsReadOnly = string.IsNullOrEmpty (unavailabilityId) && isEditMode;
            Title = IsReadOnly ? "Booking" : "Unavailability";

            if (isEditMode)
            {
                StartTimestampOfSelectedOccurrence = startTime;
                UnavailabilityId = unavailabilityId;
                EventTitle = title;
                ParkingId = parkingId;
                StartDate = startTime.UnixTimeStampToDateTime();
                EndDate = endTime.UnixTimeStampToDateTime();
                if (!string.IsNullOrEmpty(repeat))
                {
                    mOldRepeat = repeat.Trim().ToLowerInvariant();
                    var repeatList = new List<string>(AppConstants.Repeats);
                    var repeatType = repeatList.FirstOrDefault(x => x.Trim().ToLowerInvariant().Equals(repeat.Trim().ToLowerInvariant()));
                    if (repeatType != null)
                        SelectedRepeat = repeatType;
                }

                Times = !string.IsNullOrEmpty(time) ? time : string.Empty;
            }

        }

        #endregion

        public IAddEventView View { get; set; }

        #region Properties

        #region StartTimestampOfSelectedOccurrence

        public long StartTimestampOfSelectedOccurrence { get; set; }

        #endregion

        #region UnavailabilityId

        private string mUnavailabilityId = string.Empty;

        public string UnavailabilityId
        {
            get
            {
                return mUnavailabilityId;
            }
            set
            {
                mUnavailabilityId = value;
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

        #region Title

        private string mTitle = string.Empty;

        public string Title
        {
            get
            {
                return mTitle;
            }
            set
            {
                mTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ButtonTitle

        private string mButtonTitle = string.Empty;

        public string ButtonTitle
        {
            get
            {
                return mButtonTitle;
            }
            set
            {
                mButtonTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EventId

        private string mEventId = string.Empty;

        public string EventId
        {
            get
            {
                return mEventId;
            }
            set
            {
                mEventId = value;
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

        #region EventTitle

        private string mEventTitle = string.Empty;

        public string EventTitle
        {
            get
            {
                return mEventTitle;
            }
            set
            {
                mEventTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region StartDate

        //private DateTime mStartDate = DateTime.Now;
        private DateTime mStartDate = new DateTime(DateTime.UtcNow.ToLocalTime().Year, DateTime.UtcNow.ToLocalTime().Month, DateTime.UtcNow.ToLocalTime().Day, 12, 0, 0);

        public DateTime StartDate
        {
            get
            {
                return mStartDate;
            }
            set
            {
                mStartDate = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EndDate

        //private DateTime mEndDate = DateTime.Now;
        private DateTime mEndDate = new DateTime(DateTime.UtcNow.ToLocalTime().Year, DateTime.UtcNow.ToLocalTime().Month, DateTime.UtcNow.ToLocalTime().Day, 13, 0, 0);

        public DateTime EndDate
        {
            get
            {
                return mEndDate;
            }
            set
            {
                mEndDate = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Times

        private string mTimes = "1";

        public string Times
        {
            get
            {
                return mTimes;
            }
            set
            {
                mTimes = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region RepeatSource

        private string[] mRepeatSource = AppConstants.RepeatsSource;

        public string[] RepeatSource
        {
            get { return mRepeatSource; }
            set
            {
                mRepeatSource = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region RepeatSelectedTemp

        private string mRepeatSelectedTemp = AppConstants.RepeatsSource[0];

        public string RepeatSelectedTemp
        {
            get
            {
                return mRepeatSelectedTemp;
            }
            set
            {
                mRepeatSelectedTemp = value;
                var list = new List<string>(AppConstants.RepeatsSource);
                int index = list.IndexOf(value);
                SelectedRepeat = AppConstants.Repeats[index];
                IsShowTimes = (index == 0) ? false : true;
                RaisePropertyChanged();
            }

        }

        #endregion

        #region SelectedRepeat

        private string mSelectedRepeat = AppConstants.Repeats[0];

        public string SelectedRepeat
        {
            get
            {
                return mSelectedRepeat;
            }
            set
            {
                mSelectedRepeat = value;

                var list = new List<string>(AppConstants.Repeats);
                int index = list.IndexOf(value);
                if (AppConstants.RepeatsSource[index] != RepeatSelectedTemp)
                {
                    RepeatSelectedTemp = AppConstants.RepeatsSource[index];
                }


                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowTimes

        private bool mIsShowTimes = false;

        public bool IsShowTimes
        {
            get
            {
                return mIsShowTimes;
            }
            set
            {
                mIsShowTimes = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsReadOnly

        private bool mIsReadOnly = false;

        public bool IsReadOnly {
            get {
                return mIsReadOnly;
            }
            set {
                mIsReadOnly = value;
                RaisePropertyChanged ();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region AddEventCommand

        private MvxCommand mAddEventCommand = null;

        public MvxCommand AddEventCommand
        {
            get
            {
                if (mAddEventCommand == null)
                {
                    mAddEventCommand = new MvxCommand(this.AddEvent);
                }
                return mAddEventCommand;
            }
        }

        private async void AddEvent()
        {
            if (IsReadOnly) {
                return;
            }
                
            if (string.IsNullOrEmpty(EventTitle) || string.IsNullOrEmpty(EventTitle.Trim()))
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Event title must not be empty"));
                return;
            }

            if (EndDate.CompareTo(DateTime.UtcNow.ToLocalTime()) != 1 || StartDate.CompareTo(DateTime.UtcNow.ToLocalTime()) != 1)
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "End Date or Start Date must be later than now"));
                return;
            }

            if (EndDate.CompareTo(StartDate) == 0)
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Start time & end time must be different when the dates are equal"));
                return;
            }

            if (EndDate.CompareTo(StartDate) > 0)
            {
                if (BaseView != null && BaseView.CheckInternetConnection())
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                    if (IsEditMode)
                    {
                        if (mOldRepeat.Equals("once"))
                        {
                            var result =
                                await mApiService.PutUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, UnavailabilityId, EventTitle, Helpers.TimestampHelpers.DateTimeToTimeStamp(StartDate), Helpers.TimestampHelpers.DateTimeToTimeStamp(EndDate),
                                    new Periodicity()
                                    {

                                    }, "all", true, StartTimestampOfSelectedOccurrence);

                            if (result != null)
                            {
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}\n{1}",result.Result, result.ErrorCode )));
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}: {1}", result.Result, result.ErrorCode)));
                                if (result.Response.Status.Equals("success"))
                                {
                                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                                }
                                else
                                {
                                    Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                                }
                            }
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                            mCacheService.NeedReloadEvent = true;
                            Close(this);
                            return;
                        }
                        //edit unavaiability
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("EditEventText"), SharedTextSource.GetText("AreYouSureEventText"), SharedTextSource.GetText("CancelText"), null, new string[] { SharedTextSource.GetText("EditFutureEventsText"), SharedTextSource.GetText("EditEventOnlyText") },
                                async () =>
                                {
                                    var result =
                                        await mApiService.PutUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, UnavailabilityId, EventTitle, Helpers.TimestampHelpers.DateTimeToTimeStamp(StartDate), Helpers.TimestampHelpers.DateTimeToTimeStamp(EndDate),
                                            new Periodicity()
                                            {
                                                Repeat = SelectedRepeat,
                                                OccurrencesAmount = Times,
                                                Exceptions = new List<PeriodicityException>()
                                                {

                                                }
                                            }, "all", false, StartTimestampOfSelectedOccurrence);

                                    if (result != null)
                                    {
                                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}\n{1}", result.Result, result.ErrorCode)));
                                        if (result.Response.Status.Equals("success"))
                                        {
                                            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                                        }
                                        else
                                        {
                                            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                                        }
                                    }

                                    mCacheService.NeedReloadEvent = true;
                                    Close(this);
                                }, async () =>
                                {
                                    var result =
                                        await mApiService.PutUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, UnavailabilityId, EventTitle, Helpers.TimestampHelpers.DateTimeToTimeStamp(StartDate), Helpers.TimestampHelpers.DateTimeToTimeStamp(EndDate),
                                            new Periodicity()
                                            {

                                            }, "all", true, StartTimestampOfSelectedOccurrence);

                                    if (result != null)
                                    {
                                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}\n{1}",result.Result, result.ErrorCode )));
                                        //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}: {1}", result.Result, result.ErrorCode)));
                                        if (result.Response.Status.Equals("success"))
                                        {
                                            Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                                        }
                                        else
                                        {
                                            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                                        }
                                    }

                                    mCacheService.NeedReloadEvent = true;
                                    Close(this);
                                }));
                    }
                    else
                    {
                        //add unavaiability
                        var result =
                            await mApiService.CreateUnavailabilities(mCacheService.CurrentUser.UserId, mCacheService.ParkingId, EventTitle, Helpers.TimestampHelpers.DateTimeToTimeStamp(StartDate), Helpers.TimestampHelpers.DateTimeToTimeStamp(EndDate),
                                new Periodicity()
                                {
                                    Repeat = SelectedRepeat,
                                    OccurrencesAmount = Times
                                }, "all");

                        if (result != null)
                        {
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                            //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Equals("success") ? "Success" : string.Format("{0}: {1}", result.Response.Result, result.Response.ErrorCode)));
                            if (result.Response.Status.Equals("success"))
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                            }
                            else
                            {
                                if (result.ApiError.Status.Equals ("999")) 
                                    Mvx.Resolve<IMvxMessenger> ().Publish (new AlertMessage (this, string.Empty, string.Format ("{0}", result.Response.ErrorCode), "Ok", null));
                                else 
                                    Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));

                            }
                        }

                        mCacheService.NeedReloadEvent = true;
                        Close(this);
                    }



                    Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
                }
                else
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
                }

            }
            else
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "End Date must be later than Start Date"));
        }

        #endregion

        #region SelectStartDateCommand

        private MvxCommand mSelectStartDateCommand;

        public MvxCommand SelectStartDateCommand
        {
            get
            {
                if (mSelectStartDateCommand == null)
                {
                    mSelectStartDateCommand = new MvxCommand(this.SelectStartDate);
                }
                return mSelectStartDateCommand;
            }
        }

        private void SelectStartDate()
        {
            if (IsReadOnly) {
                return;
            }

            if (View != null)
                View.ShowStartDatePicker();
        }

        #endregion

        #region SelectStartTimeCommand

        private MvxCommand mSelectStartTimeCommand;

        public MvxCommand SelectStartTimeCommand
        {
            get
            {
                if (mSelectStartTimeCommand == null)
                {
                    mSelectStartTimeCommand = new MvxCommand(this.SelectStartTime);
                }
                return mSelectStartTimeCommand;
            }
        }

        private void SelectStartTime()
        {
            if (IsReadOnly) {
                return;
            }

            if (View != null)
                View.ShowStartTimePicker();
        }

        #endregion

        #region SelectEndDateCommand

        private MvxCommand mSelectEndDateCommand;

        public MvxCommand SelectEndDateCommand
        {
            get
            {
                if (mSelectEndDateCommand == null)
                {
                    mSelectEndDateCommand = new MvxCommand(this.SelectEndDate);
                }
                return mSelectEndDateCommand;
            }
        }

        private void SelectEndDate()
        {
            if (IsReadOnly) {
                return;
            }

            if (View != null)
                View.ShowEndDatePicker();
        }

        #endregion

        #region SelectEndTimeCommand

        private MvxCommand mSelectEndTimeCommand;

        public MvxCommand SelectEndTimeCommand
        {
            get
            {
                if (mSelectEndTimeCommand == null)
                {
                    mSelectEndTimeCommand = new MvxCommand(this.SelectEndTime);
                }
                return mSelectEndTimeCommand;
            }
        }

        private void SelectEndTime()
        {
            if (IsReadOnly) {
                return;
            }

            if (View != null)
                View.ShowEndTimePicker();
        }

        #endregion

        #region ShowRepeatPickerCommand

        private MvxCommand mShowRepeatPickerCommand = null;

        public MvxCommand ShowRepeatPickerCommand
        {
            get
            {
                if (mShowRepeatPickerCommand == null)
                {
                    mShowRepeatPickerCommand = new MvxCommand(this.ShowRepeatPicker);
                }
                return mShowRepeatPickerCommand;
            }
        }

        private void ShowRepeatPicker()
        {
            if (IsReadOnly) {
                return;
            }

            if (View != null)
                View.ShowRepeatPicker();
        }

        #endregion

        #region DeleteCommand

        private MvxCommand mDeleteCommand = null;

        public MvxCommand DeleteCommand
        {
            get
            {
                if (mDeleteCommand == null)
                {
                    mDeleteCommand = new MvxCommand(this.DeleteEvent);
                }
                return mDeleteCommand;
            }
        }

        private void DeleteEvent()
        {
            if (IsReadOnly) {
                return;
            }

            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("DeleteEventText"), SharedTextSource.GetText("AreYouSureDeleteEventText"), SharedTextSource.GetText("CancelText"), null, new string[] { SharedTextSource.GetText("ConfirmText") },
                    async () =>
                    {
                        if (BaseView != null && BaseView.CheckInternetConnection())
                        {
                            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                            var result = await mApiService.DeleteUnavailabilities(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId, UnavailabilityId);
                            if (result != null)
                            {
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result));
                                //Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Result.Equals("success") ? result.Result : string.Format("{0}: {1}", result.Result, result.ErrorCode)));
                                if (result)
                                {
                                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "Success"));
                                    Close(this);
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
                    }));
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

