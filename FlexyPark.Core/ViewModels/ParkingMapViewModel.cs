using System;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using FlexyPark.Core.Models;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;
using FlexyPark.Core.Services;
using Flurl.Util;
using System.Threading.Tasks;
using FlexyPark.Core.Helpers;

namespace FlexyPark.Core.ViewModels
{
    public interface IParkingMapView
    {
        void ShowRoutesPopup();
        void ChangeNavigationMode();
        void StartNavigation();
        void EndNavigation();
        void ReCenter();
        void ChangeBarButton();
    }

    public class ParkingMapViewModel : BaseViewModel
    {
        private readonly IPlatformService mPlatformService;

        #region Constructors

        public ParkingMapViewModel( IPlatformService platformService , IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
            this.mPlatformService = platformService;
        }

        #endregion

        #region Init

        public async void Init(ParkingStatus status, int timeLeft, long startTime)
        {
            Status = status;
            TotalParkingTime = timeLeft;
            Title = startTime.UnixTimeStampToDateTime().ToString("MM/dd/yyyy");

            await Task.Delay(100);
            OverviewResumeTitle = TextSource.GetText("OverviewText");

            if (View != null)
                View.ChangeBarButton();
        }

        #endregion

        public IParkingMapView View { get; set; }

        #region Properties

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

        #region OverviewResumeTitle

        private string mOverviewResumeTitle;

        public string OverviewResumeTitle {
            get {
                return mOverviewResumeTitle;
            }
            set {
                mOverviewResumeTitle = value;
                RaisePropertyChanged ();
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

        #region IsDrawedStreet

        private bool mIsDrawedStreet = false;

        public bool IsDrawedStreet
        {
            get { return mIsDrawedStreet; }
            set
            {
                mIsDrawedStreet = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Routes

        private ObservableCollection<RouteItem> mRoutes = new ObservableCollection<RouteItem>();

        public ObservableCollection<RouteItem> Routes
        {
            get
            {
                return mRoutes;
            }
            set
            {
                mRoutes = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region ExpectedTime

        private TimeSpan mExpectedTime;

        public TimeSpan ExpectedTime
        {
            get
            {
                return mExpectedTime;
            }
            set
            {
                mExpectedTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region TotalDistance

        private double mTotalDistance;

        public double TotalDistance
        {
            get
            {
                return mTotalDistance;
            }
            set
            {
                mTotalDistance = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region HasStaredNavigation

        private bool mHasStaredNavigation;

        public bool HasStaredNavigation
        {
            get
            {
                return mHasStaredNavigation;
            }
            set
            {
                mHasStaredNavigation = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsNavigating

        private bool mIsNavigating;

        public bool IsNavigating
        {
            get
            {
                return mIsNavigating;
            }
            set
            {
                mIsNavigating = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region IsShowReCenter

        private bool mIsShowReCenter = false;

        public bool IsShowReCenter
        {
            get { return mIsShowReCenter; }
            set
            {
                mIsShowReCenter = value && IsNavigating;
               // mIsShowReCenter = value ;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region ShowRouteInfoCommand

        private MvxCommand mShowRouteInfoCommand = null;

        public MvxCommand ShowRouteInfoCommand
        {
            get
            {
                if (mShowRouteInfoCommand == null)
                {
                    mShowRouteInfoCommand = new MvxCommand(this.ShowRouteInfo);
                }
                return mShowRouteInfoCommand;
            }
        }

        private void ShowRouteInfo()
        {
            if (View != null)
                View.ShowRoutesPopup();
        }

        #endregion

        #region StartNavigateCommand

        private MvxCommand mStartNavigateCommand;

        public MvxCommand StartNavigateCommand
        {
            get
            {
                if (mStartNavigateCommand == null)
                {
                    mStartNavigateCommand = new MvxCommand(this.StartNavigate);
                }
                return mStartNavigateCommand;
            }
        }


        private void StartNavigate()
        {
            if (((mPlatformService.OS == OS.Touch) && (Routes == null || Routes.Count == 0)) || (mPlatformService.OS == OS.Droid && !IsDrawedStreet))
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, SharedTextSource.GetText("CanNotFindRoutesText")));
                return;
            }

            HasStaredNavigation = true;
            IsNavigating = true;
            if (View != null)
            {
                View.StartNavigation();
                View.ChangeBarButton();
            }
        }

        #endregion

        #region ChangeNavigationModeCommand

        private MvxCommand mChangeNavigationModeCommand;

        public MvxCommand ChangeNavigationModeCommand {
            get {
                if (mChangeNavigationModeCommand == null) {
                    mChangeNavigationModeCommand = new MvxCommand (this.ChangeNavigationMode);
                }
                return mChangeNavigationModeCommand;
            }
        }

        private void ChangeNavigationMode ()
        {
            if (OverviewResumeTitle == TextSource.GetText("OverviewText")) {
                OverviewResumeTitle = TextSource.GetText("ResumeText");
            } else {
                OverviewResumeTitle = TextSource.GetText("OverviewText");
            }

            IsNavigating = !IsNavigating;
            if (View != null)
                View.ChangeNavigationMode();
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
            IsNavigating = false;
            if (View != null)
                View.EndNavigation();
        }

        #endregion

        #region ReCenterCommand

        private MvxCommand mReCenterCommand;

        public MvxCommand ReCenterCommand
        {
            get
            {
                if (mReCenterCommand == null)
                {
                    mReCenterCommand = new MvxCommand(this.ReCenter);
                }
                return mReCenterCommand;
            }
        }

        private void ReCenter()
        {
           
            if (View != null)
            {
                View.ReCenter();
            }
        }

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}

