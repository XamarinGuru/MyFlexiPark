using System;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using FlexyPark.Core.Services;
using Cirrious.MvvmCross.Localization;
using FlexyPark.Core.Helpers;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.Core.ViewModels
{
    public class ReservationItemViewModel : MvxViewModel
    {
        private MyReservationsViewModel mParentVM;
       
        #region Constructors

        public ReservationItemViewModel(MyReservationsViewModel parentVM)
        {
            this.mParentVM = parentVM;
        }

        #endregion

        #region Init

        public void Init()
        {
        }

        #endregion

        #region Properties

        #region TextSource

        private IMvxLanguageBinder mTextSource = null;

        public IMvxLanguageBinder TextSource
        {
            get{
                if(mTextSource == null)
                    mTextSource = new MvxLanguageBinder(AppConstants.NameSpace, GetType().Name);

                return mTextSource;
            }
            set{
                mTextSource = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Reservation

        private Reservation mReservation = null;

        public Reservation Reservation
        {
            get
            {
                return mReservation;
            }
            set
            {
                mReservation = value;
                RaisePropertyChanged();

                StartTime = Reservation.StartTimestamp.UnixTimeStampToDateTime();
                EndTime = Reservation.EndTimestamp.UnixTimeStampToDateTime();

                IsShowCancelButton = DateTime.Compare(StartTime, DateTime.UtcNow.AddHours(24)) == 1;
            }
        }

        #endregion

        #region StartTime

        private DateTime mStartTime = DateTime.UtcNow.ToLocalTime();

        public DateTime StartTime
        {
            get
            {
                return mStartTime;
            }
            set
            {
                mStartTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EndTime

        private DateTime mEndTime = DateTime.UtcNow.ToLocalTime();

        public DateTime EndTime
        {
            get
            {
                return mEndTime;
            }
            set
            {
                mEndTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowCancelButton

        private bool mIsShowCancelButton = false;

        public bool IsShowCancelButton
        {
            get
            {
                return mIsShowCancelButton;
            }
            set
            {
                mIsShowCancelButton = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region CancelReservationCommand

        private MvxCommand mCancelReservationCommand = null;

        public MvxCommand CancelReservationCommand
        {
            get
            {
                if (mCancelReservationCommand == null)
                {
                    mCancelReservationCommand = new MvxCommand(this.CancelReservation);
                }
                return mCancelReservationCommand;
            }
        }

        private void CancelReservation()
        {
            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(mParentVM, mParentVM.SharedTextSource.GetText("CancelReservationText"), mParentVM.SharedTextSource.GetText("AreYouSureCancelReservationText"), mParentVM.SharedTextSource.GetText("NoText"), null,
                new string[] {mParentVM.SharedTextSource.GetText("YesText")},
                ()=>{
                //delete vehicle
                mParentVM.DeleteReservation();
            })); 
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

