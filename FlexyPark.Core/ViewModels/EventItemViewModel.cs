using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Linq;
using FlexyPark.Core.Services;
using FlexyPark.Core.Helpers;

namespace FlexyPark.Core.ViewModels
{
    public class EventItemViewModel : MvxViewModel
    {
        private readonly AddSpotCalendarViewModel mParentVM;

        #region Constructors

        public EventItemViewModel(AddSpotCalendarViewModel parentVm)
        {
            mParentVM = parentVm;
        }

        #endregion

        #region Properties

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

        #region IsSelected

        private bool mIsSelected = false;

        public bool IsSelected
        {
            get
            {
                return mIsSelected;
            }
            set
            {
                mIsSelected = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Unavaiability

        private OwnerUnavailability mUnavaiability = null;

        public OwnerUnavailability Unavaiability
        {
            get
            {
                return mUnavaiability;
            }
            set
            {
                mUnavaiability = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Booking

        private Reservation mBooking;

        public Reservation Booking
        {
            get
            {
                return mBooking;
            }
            set
            {
                mBooking = value;
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

        #region CalendarID

        private string mCalendarID = string.Empty;

        public string CalendarID
        {
            get
            {
                return mCalendarID;
            }
            set
            {
                mCalendarID = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Summary

        private string mSummary = string.Empty;

        public string Summary
        {
            get
            {
                return mSummary;
            }
            set
            {
                mSummary = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region StartDate

        private DateTime mStartDate = DateTime.UtcNow.ToLocalTime();

        public DateTime StartDate
        {
            get
            {
                if (Unavaiability != null)
                    return Unavaiability.StartTimestamp.UnixTimeStampToDateTime();
                else if (Booking != null)
                    return Booking.StartTimestamp.UnixTimeStampToDateTime();
                else
                    return DateTime.UtcNow.ToLocalTime();
            }
            /*set
            {
                mStartDate = value;
                RaisePropertyChanged();
            }*/
        }

        #endregion

        #region EndDate

        private DateTime mEndDate = DateTime.UtcNow.ToLocalTime();

        public DateTime EndDate
        {
            get
            {
                if (Unavaiability != null)
                    return Unavaiability.EndTimestamp.UnixTimeStampToDateTime();
                else if (Booking != null)
                    return Booking.EndTimestamp.UnixTimeStampToDateTime();
                else
                    return DateTime.UtcNow.ToLocalTime();
            }
            /*set
            {
                mEndDate = value;
                RaisePropertyChanged();
            }*/
        }

        #endregion

        #endregion

        #region Commands

        #region EditEventCommand

        private MvxCommand mEditEventCommand = null;

        public MvxCommand EditEventCommand
        {
            get
            {
                if (mEditEventCommand == null)
                {
                    mEditEventCommand = new MvxCommand(this.EditEvent);
                }
                return mEditEventCommand;
            }
        }

        private void EditEvent()
        {
            if (mParentVM != null && mParentVM.View != null)
            {
                mParentVM.EventItemClickedCommand.Execute(this);

            }
            //ShowViewModel<AddEventViewModel>(new { isEditMode = true, title = Unavaiability.Title, startTime = Unavaiability.StartTimestamp, endTime = Unavaiability.EndTimestamp, eventId = EventId, calendarId = CalendarID });
        }

        #endregion

        #region DeleteEventCommand

        private MvxCommand mDeleteEventCommand = null;

        public MvxCommand DeleteEventCommand
        {
            get
            {
                if (mDeleteEventCommand == null)
                {
                    mDeleteEventCommand = new MvxCommand(this.DeleteEvent);
                }
                return mDeleteEventCommand;
            }
        }

        private void DeleteEvent()
        {
            if (mParentVM != null)
                mParentVM.DeleteEventCommand.Execute(this);

            //Mvx.Resolve<IMvxMessenger>()
            //    .Publish(new AlertMessage(this.mParentVM, mParentVM.SharedTextSource.GetText("DeleteEventText"), mParentVM.SharedTextSource.GetText("AreYouSureDeleteEventText"),
            //        mParentVM.SharedTextSource.GetText("NoText"), null, new string[] { mParentVM.SharedTextSource.GetText("DeleteEventOnlyText"), mParentVM.SharedTextSource.GetText("DeleteFutureEventsText") },
            //        () =>
            //        {
            //            // DoDeleteEvent();
            //            mParentVM.DeleteUnavaiability();
            //        },
            //        () =>
            //        {
            //            mParentVM.DeleteUnavaiability();
            //        }));

        }

        #endregion

        #endregion

        #region Methods



        #endregion
    }
}

