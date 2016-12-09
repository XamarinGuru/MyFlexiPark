using System;
using Cirrious.MvvmCross.ViewModels;
using FlexyPark.Core.Services;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using System.Collections.Generic;
using FlexyPark.Core.Helpers;
using System.Globalization;

namespace FlexyPark.Core.ViewModels
{
    public interface IParkingSummaryView
    {
        T GetPreference<T>(string key);

        void NavigateUsingWaze(double lat, double lng, int zoomLevel = 1);

        void NavigateUsingGoogleMaps(double destinationLat, double destinationLng, int zoomLevel = 1, DirectionsMode directionsMode = DirectionsMode.Driving);

        void NavigateUsingNativeMap(double destinationLat, double destinationLng, int zoomLevel = 1, DirectionsMode directionsMode = DirectionsMode.Driving);

        void NavigateUsingNavmii(double lat, double lng);
    }

    public class ParkingSummaryViewModel : BaseViewModel
    {
        public readonly ParkingReservedViewModel mParentViewModel;
        private readonly IPlatformService mPlatformService;

        #region Constructors

        public ParkingSummaryViewModel(ParkingReservedViewModel parentViewModel, ParkingStatus status, IPlatformService platformService, IApiService apiService, ICacheService cacheService)
            : base(apiService, cacheService)
        {
            this.mParentViewModel = parentViewModel;
            this.Status = status;
            this.mPlatformService = platformService;
            Reservation = mCacheService.CurrentReservation;

			IsShowLeaveReportButton = Reservation.LeavingStatus == null || Reservation.LeavingStatus.Equals ("na");
        }

        #endregion

        #region Init

        public void Init()
        {
        }

        #endregion

        public IParkingSummaryView View { get; set; }

        #region Properties

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

        #region IsShowOfferedTime

        private bool mIsShowOfferedTime = false;

        public bool IsShowOfferedTime
        {
            get
            {
                return mIsShowOfferedTime;
            }
            set
            {
                mIsShowOfferedTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region OfferedTime

        /// <summary>
        /// The offered time ( in minutes ).
        /// </summary>
        private int mOfferedTime = 5;

        public int OfferedTime
        {
            get
            {
                return mOfferedTime;
            }
            set
            {
                mOfferedTime = value;
                RaisePropertyChanged();

                if (value == 0)
                    IsShowOfferedTime = false;
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

        #region IsShowLeaveReportButton

        private bool mIsShowLeaveReportButton = false;

        public bool IsShowLeaveReportButton {
            get {
                return mIsShowLeaveReportButton;
            }
            set {
                mIsShowLeaveReportButton = value;
                RaisePropertyChanged ();
            }
        }

        #endregion


        #endregion

        #region Commands

        #region NavigateCommand

        private MvxCommand mNavigateCommand = null;

        public MvxCommand NavigateCommand
        {
            get
            {
                if (mNavigateCommand == null)
                {
                    mNavigateCommand = new MvxCommand(this.Navigate);
                }
                return mNavigateCommand;
            }
        }

        private async void Navigate()
        {
            if (IsReadOnly)
                return;

            var parkingLat = double.Parse(Reservation.Parking.Latitude, CultureInfo.InvariantCulture);
            var parkingLng = double.Parse(Reservation.Parking.Longitude, CultureInfo.InvariantCulture);
            if (View != null)
            {
                if (mPlatformService.OS == OS.Touch) //iOS
                {
                    var wazeInstalled = View.GetPreference<bool>(AppConstants.Waze);
                    var ggMapsInstalled = View.GetPreference<bool>(AppConstants.GoogleMaps);
                    var navmiiInstalled = View.GetPreference<bool>(AppConstants.Navmii);
                    var options = new List<string>();
                    var actions = new List<Action>();

                    if (wazeInstalled)
                    {
                        options.Add(TextSource.GetText("UsingWazeText"));
                        actions.Add(() =>
                            {
                                if (View != null)
                                    View.NavigateUsingWaze(parkingLat, parkingLng);
                            });
                    }
                    if (ggMapsInstalled)
                    {
                        options.Add(TextSource.GetText("UsingGoogleMapsText"));
                        actions.Add(() =>
                            {
                                if (View != null)
                                    View.NavigateUsingGoogleMaps(parkingLat, parkingLng);
                            });
                    }
                    if (navmiiInstalled)
                    {
                        options.Add(TextSource.GetText("UsingNavmiiText"));
                        actions.Add(() =>
                            {
                                if (View != null)
                                    View.NavigateUsingNavmii(parkingLat, parkingLng);
                            });
                    }

                    options.Add(TextSource.GetText("UsingNativeMapText"));
                    actions.Add(async () =>
                        {
                            //mParentViewModel.ShowMapTab();
                            ShowViewModel<ParkingMapViewModel>(new {status = mParentViewModel.Status, timeLeft = mParentViewModel.TotalParkingTime, startTime = (Reservation != null) ? Convert.ToInt64(Reservation.StartTimestamp, CultureInfo.InvariantCulture) : 0 });
                            await Task.Delay(200);

                            if (Reservation != null && Reservation.Parking != null)
                                Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, Convert.ToDouble(Reservation.Parking.Latitude, CultureInfo.InvariantCulture), Convert.ToDouble(Reservation.Parking.Longitude, CultureInfo.InvariantCulture)));
                            else
                                Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, 50.673859, 4.615169));
                        });

                    if (options.Count == 1 && options.Contains(TextSource.GetText("UsingNativeMapText")))
                    {
                        //mParentViewModel.ShowMapTab();
                        ShowViewModel<ParkingMapViewModel>(new {status = mParentViewModel.Status, timeLeft = mParentViewModel.TotalParkingTime, startTime = (Reservation != null) ? Convert.ToInt64(Reservation.StartTimestamp, CultureInfo.InvariantCulture) : 0 });
                        await Task.Delay(200);

                        if (Reservation != null && Reservation.Parking != null)
                            Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, Convert.ToDouble(Reservation.Parking.Latitude, CultureInfo.InvariantCulture), Convert.ToDouble(Reservation.Parking.Longitude, CultureInfo.InvariantCulture)));
                        else
                            Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, 50.673859, 4.615169));
                        return;
                    }

                    if (options.Count > 1)
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, TextSource.GetText("PleaseSelectText"), TextSource.GetText("WhichNavigationTypeText"), TextSource.GetText("CancelText"), null, 
                                options.ToArray(),
                                actions.ToArray()
                            ));
                    }
                }
                else if (mPlatformService.OS == OS.Droid) //Android
                {
                    if (View.GetPreference<bool>(AppConstants.Waze))
                    {
                        Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, TextSource.GetText("PleaseSelectText"), TextSource.GetText("WhichNavigationTypeText"), TextSource.GetText("CancelText"), null, 
                                new string[]{ TextSource.GetText("UsingWazeText"), TextSource.GetText("UsingNativeMapText") },
                                () =>
                                {
                                    if (View != null)
                                    View.NavigateUsingWaze(parkingLat, parkingLng);
                                },
                                async() =>
                                {
                                    ShowViewModel<ParkingMapViewModel>(new {status = mParentViewModel.Status, timeLeft = mParentViewModel.TotalParkingTime, startTime = (Reservation != null) ? Convert.ToInt64(Reservation.StartTimestamp, CultureInfo.InvariantCulture) : 0 });
                                    await Task.Delay(200);

                                if (Reservation != null && Reservation.Parking != null)
                                        Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, Convert.ToDouble(Reservation.Parking.Latitude, CultureInfo.InvariantCulture), Convert.ToDouble(Reservation.Parking.Longitude, CultureInfo.InvariantCulture)));
                                    else
                                        Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, parkingLng, 4.615169));
                                }));
                    }
                    else
                    {                        
                        ShowViewModel<ParkingMapViewModel>(new {status = mParentViewModel.Status, timeLeft = mParentViewModel.TotalParkingTime, startTime = (Reservation != null) ? Convert.ToInt64(Reservation.StartTimestamp, CultureInfo.InvariantCulture) : 0 });
                        await Task.Delay(200);

                        if (Reservation != null && Reservation.Parking != null)
                            Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, Convert.ToDouble(Reservation.Parking.Latitude, CultureInfo.InvariantCulture), Convert.ToDouble(Reservation.Parking.Longitude, CultureInfo.InvariantCulture)));
                        else
                            Mvx.Resolve<IMvxMessenger>().Publish(new NavigateMapMessage(this, 50.673859, 4.615169));
                    }
                }
            }

        }

        #endregion

        #region GotoLeaveParkingCommand

        private MvxCommand mGotoLeaveParkingCommand = null;

        public MvxCommand GotoLeaveParkingCommand
        {
            get
            {
                if (mGotoLeaveParkingCommand == null)
                {
                    mGotoLeaveParkingCommand = new MvxCommand(this.GotoLeaveParking);
                }
                return mGotoLeaveParkingCommand;
            }
        }

        private void GotoLeaveParking()
        {
            /*Mvx.Resolve<IMvxMessenger>().Publish(new AlertMessage(this, SharedTextSource.GetText("WarningText"), SharedTextSource.GetText("LeaveThisParkingText"), SharedTextSource.GetText("NoText"), null, new string[] { SharedTextSource.GetText("YesText") }, 
                ()=>
                {
                    mParentViewModel.NeedRelease = true;
                    ShowViewModel<LeaveParkingViewModel>();
                    Close(this);
                }
            ));*/
            if (IsReadOnly)
                return;

            ShowViewModel<LeaveParkingViewModel>();
        }

        #endregion

        #region ReportProblemCommand

        private MvxCommand mReportProblemCommand = null;

        public MvxCommand ReportProblemCommand
        {
            get
            {
                if (mReportProblemCommand == null)
                {
                    mReportProblemCommand = new MvxCommand(this.ReportProblem);
                }
                return mReportProblemCommand;
            }
        }

        private void ReportProblem()
        {
            if (IsReadOnly)
                return;

            mParentViewModel.NeedRelease = false;
            ShowViewModel<ReportSelectionViewModel>();
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

