using System;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using FlexyPark.Core.Services;
using System.Globalization;
using Cirrious.CrossCore;

namespace FlexyPark.Core.ViewModels
{
    public class ExtendParkingTimeViewModel : BaseViewModel
    {
        #region Constructors

        public ExtendParkingTimeViewModel(IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public void Init(string parkingId, long startTimeStamp, int timeLeft, string plateNumber)
        {
            //timeLeft == 0 means that in ReservedMode
            if (mCacheService.Extends != null)
                ExtendTimes = new ObservableCollection<ExtendTimeItemViewModel>(mCacheService.Extends);

            TotalParkingTime = timeLeft;
            StartTimeStamp = startTimeStamp;
            ParkingId = parkingId;
            PlateNumber = plateNumber;

            Mvx.Resolve<ICacheService>().SharedTextSource = SharedTextSource;

        }

        #endregion

        #region Properties

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

        #region TotalParkingTime

        private int mTotalParkingTime = 0;

        public int TotalParkingTime
        {
            get
            {
                return mTotalParkingTime;
            }
            set
            {
                mTotalParkingTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ExtendTimeLists

        private ObservableCollection<ExtendTimeItemViewModel> mExtendTimes = new ObservableCollection<ExtendTimeItemViewModel>();

        public ObservableCollection<ExtendTimeItemViewModel> ExtendTimes
        {
            get
            {
                return mExtendTimes;
            }
            set
            {
                mExtendTimes = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region StartTimeStamp

        private long mStartTimeStamp = 0;

        public long StartTimeStamp
        {
            get
            {
                return mStartTimeStamp;
            }
            set
            {
                mStartTimeStamp = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region PlateNumber

        private string mPlateNumber = string.Empty;

        public string PlateNumber
        {
            get
            {
                return mPlateNumber;
            }
            set
            {
                mPlateNumber = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoExtendParkingTimeConfirmCommand

        private MvxCommand<ExtendTimeItemViewModel> mGotoExtendParkingTimeConfirmCommand = null;

        public MvxCommand<ExtendTimeItemViewModel> GotoExtendParkingTimeConfirmCommand
        {
            get
            {
                if (mGotoExtendParkingTimeConfirmCommand == null)
                {
                    mGotoExtendParkingTimeConfirmCommand = new MvxCommand<ExtendTimeItemViewModel>(this.GotoExtendParkingTimeConfirm);
                }
                return mGotoExtendParkingTimeConfirmCommand;
            }
        }

        private void GotoExtendParkingTimeConfirm(ExtendTimeItemViewModel itemVM)
        {
            ShowViewModel<ExtendParkingTimeConfirmViewModel>(new
                {
                    parkingId = ParkingId,
                    cost = itemVM.Price,
                    remainingCredits = double.Parse(mCacheService.CurrentUser.RemainingCredits, CultureInfo.InvariantCulture),
                    duration = itemVM.Hours,
                    startTimestamp = StartTimeStamp,
                    endTimestamp = StartTimeStamp + (3600 * itemVM.Hours),
                    timeLeft = TotalParkingTime,
                    plateNumber = PlateNumber
                });
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

