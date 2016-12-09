using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using FlexyPark.Core.Helpers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SQLitePCL;
using System.Globalization;

namespace FlexyPark.Core.ViewModels
{
    public interface IParkingReservedView
    {
        void StopTimer();

        void ShowMapTab();

        void Buzzing(bool isStart);

        void ChangeBarButton(bool isNavigating);
    }

    public class ParkingReservedViewModel : BaseViewModel
    {
        //private int mCounter = 0;
        private readonly IPlatformService mPlatformService;

        #region Constructors

        public ParkingReservedViewModel(IPlatformService platformService, IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
            this.mPlatformService = platformService;
           
        }

        #endregion

        #region Init

        public async void Init(ParkingStatus status, bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
            Status = status;
            SummaryVM = new ParkingSummaryViewModel(this, Status, mPlatformService, mApiService, mCacheService);
            //if (mCacheService.CurrentReservation != null)
            SummaryVM.Reservation = mCacheService.CurrentReservation;
            SummaryVM.IsReadOnly = IsReadOnly;
            //MapVM = new ParkingMapViewModel(this, mPlatformService, mApiService, mCacheService);
            SummaryVM.Init();
            //MapVM.Init();

            await Task.Delay(100);
            OverviewResumeTitle = TextSource.GetText("OverviewText");

            //TotalParkingTime = (int)(long.Parse(SummaryVM.Reservation.EndTimestamp) - long.Parse(SummaryVM.Reservation.StartTimestamp));

            CheckTotalParkingTime();
        }

        #endregion

        public IParkingReservedView View { get; set; }

        #region Properties

        #region SummaryVM

        private ParkingSummaryViewModel mSummaryVM = null;

        public ParkingSummaryViewModel SummaryVM
        {
            get
            {
                return mSummaryVM;
            }
            set
            {
                mSummaryVM = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region MapVM

        private ParkingMapViewModel mMapVM = null;

        public ParkingMapViewModel MapVM
        {
            get
            {
                return mMapVM;
            }
            set
            {
                mMapVM = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Status

        private ParkingStatus mStatus = ParkingStatus.Rented;

        public ParkingStatus Status
        {
            get
            {
                return mStatus;
            }
            set
            {
                mStatus = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region NeedRelease

        private bool mNeedRelease = false;

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

        #region TotalParkingTime

        private int mTotalParkingTime = 630;

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

                Mvx.Resolve<IMvxMessenger>().Publish(new TimeMessage(this, value));

                /*mCounter++;
                if (mCounter <= 300 && mCounter % 60 == 0)
                    SummaryVM.OfferedTime--;*/

                if (value == 600 && View != null)
                {
                    View.Buzzing(true);

//                    Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("AlertText"), SharedTextSource.GetText("YouMustLeaveText"), SharedTextSource.GetText("OKText"), 
//                            () =>
//                            {
//                                View.Buzzing(false);
//                            }, 
//                            new string[] { SharedTextSource.GetText("ExtendText") }, 
//                            () =>
//                            { 
//                                View.Buzzing(false);
//                                GotoExtendParkingTime();
//                            })); 
                }

                if (value == 0 && View != null)
                {
                    View.StopTimer();
                    /*View.Buzzing(false);*/
                    Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("TimeOutText"), SharedTextSource.GetText("YouMustLeaveText"), SharedTextSource.GetText("AlreadyLeftText"), 
                            () =>
                            { 
                                View.Buzzing(false);
                                GotoLeaveParking();
                            },
                            new string[] { SharedTextSource.GetText("ExtendText") }, 
                            () =>
                            { 
                                View.Buzzing(false);
                                GotoExtendParkingTime();
                            })); 
                }
            }
        }

        #endregion

        #region OverviewResumeTitle

        private string mOverviewResumeTitle;

        public string OverviewResumeTitle
        {
            get
            {
                return mOverviewResumeTitle;
            }
            set
            {
                mOverviewResumeTitle = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region GotoExtendParkingTimeCommand

        private MvxCommand mGotoExtendParkingTimeCommand = null;

        public MvxCommand GotoExtendParkingTimeCommand
        {
            get
            {
                if (mGotoExtendParkingTimeCommand == null)
                {
                    mGotoExtendParkingTimeCommand = new MvxCommand(this.GotoExtendParkingTime);
                }
                return mGotoExtendParkingTimeCommand;
            }
        }

        private async void GotoExtendParkingTime()
        {
            if (IsReadOnly)
                return;

            NeedRelease = false;

            //call f3 api, start with x=4, then go down until we have avaiable parkings
            //x is the time we request
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
            int x = 4;
            for (x = 4; x > 0; x--)
            {
                long startTimeStamp = long.Parse(SummaryVM.Reservation.EndTimestamp);
                long endTimeStamp = startTimeStamp + (3600 * x);

                string vehicleType = string.Empty;
                if (Status == ParkingStatus.Rented)
                    vehicleType = mCacheService.SelectedVehicle.Type;
                else
                {
                    Vehicle v = mCacheService.UserVehicles.FirstOrDefault(vehicle => vehicle.PlateNumber.Equals(SummaryVM.Reservation.PlateNumber));
                    vehicleType = v != null ? v.Type : "sedan";
                }
                var result = await mApiService.AvaiableParkingsExtensions(startTimeStamp, endTimeStamp, 5, double.Parse(SummaryVM.Reservation.Parking.Latitude, CultureInfo.InvariantCulture), double.Parse(SummaryVM.Reservation.Parking.Longitude, CultureInfo.InvariantCulture), "1000", vehicleType);
                if (result != null && result.Response.Count > 0)
                {
                    foreach (var parking in result.Response)
                    {
                        if (parking.ParkingId.Equals(SummaryVM.Reservation.Parking.ParkingId))
                        {
                            double oneHourCost = parking.Cost / x;
                            var temp = new List<ExtendTimeItemViewModel>();
                            for (int j = 1; j <= x; j++)
                            {
                                temp.Add(new ExtendTimeItemViewModel()
                                    {
                                        Hours = j,
                                        Price = oneHourCost * j,
                                        Time = (long.Parse(SummaryVM.Reservation.EndTimestamp) + (3600 * j)).UnixTimeStampToDateTime()
                                    });
                            }
                            mCacheService.Extends = temp;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    break;
                }
                else
                {
                    continue;
                }
            }    
            Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            if (x == 0)
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, string.Empty, string.Format("This parking is occupied after {0}", SummaryVM.Reservation.EndTimestamp.UnixTimeStampToDateTime().ToString("dd/MM/yyyy HH:mm")), "Find another booking", () =>
                        {
                            //pop the current booking screen
                            Close(this);
                        mCacheService.CurrentLat = double.Parse(SummaryVM.Reservation.Parking.Latitude, CultureInfo.InvariantCulture);
                        mCacheService.CurrentLng = double.Parse(SummaryVM.Reservation.Parking.Longitude, CultureInfo.InvariantCulture);
                            mCacheService.SelectedVehicle = new Vehicle(){ Type = SummaryVM.Reservation.VehicleType, PlateNumber = SummaryVM.Reservation.PlateNumber };
                            ShowViewModel<ParkingListsViewModel>(new {startTimeStamp = long.Parse(SummaryVM.Reservation.EndTimestamp)});
                        }));
            }
            else
            {
                if (Status == ParkingStatus.Rented)
                    ShowViewModel<ExtendParkingTimeViewModel>(new {parkingId = SummaryVM.Reservation.Parking.ParkingId, startTimeStamp = long.Parse(SummaryVM.Reservation.EndTimestamp), timeLeft = TotalParkingTime, plateNumber = SummaryVM.Reservation.PlateNumber});
                else
                    ShowViewModel<ExtendParkingTimeViewModel>(new {parkingId = SummaryVM.Reservation.Parking.ParkingId, startTimeStamp = long.Parse(SummaryVM.Reservation.EndTimestamp), timeLeft = 0, plateNumber = SummaryVM.Reservation.PlateNumber});
            }

        }

        #endregion

        #region GotoMenuCommand

        private MvxCommand mGotoMenuCommand = null;

        public MvxCommand GotoMenuCommand
        {
            get
            {
                if (mGotoMenuCommand == null)
                {
                    mGotoMenuCommand = new MvxCommand(this.GotoMenu);
                }
                return mGotoMenuCommand;
            }
        }

        private void GotoMenu()
        {
            ShowViewModel<MenuViewModel>(presentationFlag: PresentationBundleFlagKeys.Menu);
        }

        #endregion

        #region StartNavigationCommand

        private MvxCommand mStartNavigationCommand;

        public MvxCommand StartNavigationCommand
        {
            get
            {
                if (mStartNavigationCommand == null)
                {
                    mStartNavigationCommand = new MvxCommand(this.StartNavigation);
                }
                return mStartNavigationCommand;
            }
        }

        private void StartNavigation()
        {
            MapVM.StartNavigateCommand.Execute(null);
            if (View != null)
                View.ChangeBarButton(true);
        }

        #endregion

        #region ChangeNavigationModeCommand

        private MvxCommand mChangeNavigationModeCommand;

        public MvxCommand ChangeNavigationModeCommand
        {
            get
            {
                if (mChangeNavigationModeCommand == null)
                {
                    mChangeNavigationModeCommand = new MvxCommand(this.ChangeNavigationMode);
                }
                return mChangeNavigationModeCommand;
            }
        }

        private void ChangeNavigationMode()
        {
            if (OverviewResumeTitle == TextSource.GetText("OverviewText"))
            {
                OverviewResumeTitle = TextSource.GetText("ResumeText");
            }
            else
            {
                OverviewResumeTitle = TextSource.GetText("OverviewText");
            }

            MapVM.ChangeNavigationModeCommand.Execute();
        }

        #endregion

        #region EndNavigationCommand

        private MvxCommand mEndNavigationCommand;

        public MvxCommand EndNavigationCommand
        {
            get
            {
                if (mEndNavigationCommand == null)
                {
                    mEndNavigationCommand = new MvxCommand(this.EndNavigation);
                }
                return mEndNavigationCommand;
            }
        }

        private void EndNavigation()
        {
            if (View != null)
                View.ChangeBarButton(false);
            MapVM.EndNavigationCommand.Execute();
        }

        #endregion

        #endregion

        #region Methods

        private void GotoLeaveParking()
        {
            NeedRelease = true;
            ShowViewModel<LeaveParkingViewModel>();
        }

        public void ShowMapTab()
        {
            if (View != null)
                View.ShowMapTab();
        }

        public void CheckTotalParkingTime()
        {
            var currentBookingId = mPlatformService.GetPreference<long>(AppConstants.CurrentBookingId);
            var bookingExpiredTime = mPlatformService.GetPreference<long>(AppConstants.BookingExpiredTime);
            if ((currentBookingId == null || currentBookingId == 0) || (currentBookingId != 0 && currentBookingId != long.Parse(SummaryVM.Reservation.BookingId)))
            {
                TotalParkingTime = (int)(long.Parse(SummaryVM.Reservation.EndTimestamp) - long.Parse(SummaryVM.Reservation.StartTimestamp));
                mPlatformService.SetPreference<long>(AppConstants.BookingExpiredTime, long.Parse(SummaryVM.Reservation.EndTimestamp));
                mPlatformService.SetPreference<long>(AppConstants.CurrentBookingId, long.Parse(SummaryVM.Reservation.BookingId));
            }
            else
            {
                var currentTimeStamp = DateTime.UtcNow.ToLocalTime().DateTimeToTimeStamp();
                if (currentTimeStamp <= bookingExpiredTime)
                    TotalParkingTime = (int) (bookingExpiredTime - currentTimeStamp);
                else
                {
                    mPlatformService.SetPreference<long>(AppConstants.BookingExpiredTime, 0);
                    mPlatformService.SetPreference<long>(AppConstants.CurrentBookingId, 0);
                }
            }
        }

        #endregion
    }
}

