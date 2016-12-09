using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Helpers;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Threading.Tasks;
using FlexyPark.Core.Services;

namespace FlexyPark.Core.ViewModels
{
    public class ExtendParkingTimeConfirmViewModel : BaseViewModel
    {
        private readonly IPlatformService mPlatformService;

        #region Constructors

        public ExtendParkingTimeConfirmViewModel(IPlatformService platformService, IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
            mPlatformService = platformService;
        }

        #endregion

        #region Init

        public void Init(string parkingId, double cost, double remainingCredits, int duration, long startTimestamp, long endTimestamp, int timeLeft, string plateNumber)
        {
            ParkingId = parkingId;
            Cost = cost;
            TotalParkingTime = timeLeft;
            RemainingCredits = remainingCredits;
            Duration = duration;

            IsShowBuyCredits = RemainingCredits < Cost;

            Vehicle = mCacheService.SelectedVehicle;
            StartTimeStamp = startTimestamp;
            EndTimeStamp = endTimestamp;

            EndTime = endTimestamp.UnixTimeStampToDateTime();

            PlateNumber = plateNumber;
        }

        #endregion

        #region Properties

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

        #region NeedRelease

        private bool mNeedRelease = true;

        public bool NeedRelease
        {
            get
            {
                return mNeedRelease;
            }
            set
            {
                mNeedRelease = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Duration

        private int mDuration = 2;

        public int Duration
        {
            get
            {
                return mDuration;
            }
            set{
                mDuration = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Vehicle

        private Vehicle mVehicle = new Vehicle();

        public Vehicle Vehicle
        {
            get
            {
                return mVehicle;
            }
            set
            {
                mVehicle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region RemainingCredits

        private double mRemainingCredits = 0;

        public double RemainingCredits
        {
            get
            {
                return mRemainingCredits;
            }
            set{
                mRemainingCredits = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EndTime

        private DateTime mEndTime = DateTime.UtcNow.ToLocalTime().AddHours(2);

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

        #region IsShowBuyCredits

        private bool mIsShowBuyCredits = false;

        public bool IsShowBuyCredits
        {
            get
            {
                return mIsShowBuyCredits;
            }
            set
            {
                mIsShowBuyCredits = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Cost

        private double mCost = 0;

        public double Cost
        {
            get
            {
                return mCost;
            }
            set
            {
                mCost = value;
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

        #region EndTimeStamp

        private long mEndTimeStamp = 0;

        public long EndTimeStamp
        {
            get
            {
                return mEndTimeStamp;
            }
            set
            {
                mEndTimeStamp = value;
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

        #region PayWithCreditsCommand

        private MvxCommand mPayWithCreditsCommand = null;

        public MvxCommand PayWithCreditsCommand
        {
            get
            {
                if (mPayWithCreditsCommand == null)
                {
                    mPayWithCreditsCommand = new MvxCommand(this.PayWithCredits);
                }
                return mPayWithCreditsCommand;
            }
        }

        private async void PayWithCredits()
        {
            if (BaseView != null && BaseView.CheckInternetConnection())
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));

                var result1 = await mApiService.CreateBooking(Mvx.Resolve<ICacheService>().CurrentUser.UserId, ParkingId.ToString(), StartTimeStamp, EndTimeStamp, PlateNumber);
                if (result1 != null && result1.Response != null)
                {
                    if (result1.Response.Status.Equals("success"))
                    {
                        var result = await mApiService.PayBooking(mCacheService.CurrentUser.UserId, ParkingId.ToString(), result1.Response.BookingId);
                        if (result != null)
                        {
                            if (result.Response.Status.Equals("success"))
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, result.Response.Status));
                            }
                            else
                            {
                                Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result.Response.Status, result.Response.ErrorCode), "Ok", null));
                            }
                            Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("SuccessText"), SharedTextSource.GetText("ExtendSuccessfullyText"), SharedTextSource.GetText("OKText"),
                                () =>
                                {
                                    NeedRelease = true;
                                    mCacheService.ExtendHours = Duration;
                                    ShowViewModel<ParkingReservedViewModel>(presentationFlag: PresentationBundleFlagKeys.ParkingReserved);
                                    Close(this);
                                }));
                        }
                        else
                        {
                            Close(this);
                        }
                    }
                    else
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("{0}: {1}", result1.Response.Status, result1.Response.ErrorCode), "Ok", null));
                    }

                }

                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("TurnOnInternetText")));
            }
        }

        #endregion

        #region BuyCreditsCommand

        private MvxCommand mBuyCreditsCommand = null;

        public MvxCommand BuyCreditsCommand
        {
            get
            {
                if (mBuyCreditsCommand == null)
                {
                    mBuyCreditsCommand = new MvxCommand(this.BuyCredits);
                }
                return mBuyCreditsCommand;
            }
        }

        private void BuyCredits()
        {
            NeedRelease = false;
            //ShowViewModel<BuyCreditsViewModel>();
            ShowViewModel<RentProfileViewModel> ();
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

