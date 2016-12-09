using System;
using Cirrious.MvvmCross.ViewModels;
using System.Text.RegularExpressions;
using FlexyPark.Core.Services;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using System.Diagnostics;
using FlexyPark.Core.Helpers;
using System.Collections.ObjectModel;
using Cirrious.MvvmCross.Plugins.Messenger;
using FlexyPark.Core.Messengers;

namespace FlexyPark.Core.ViewModels
{
    public interface IParkingSearchView
    {
        /// <summary>
        /// Shows the date picker.
        /// </summary>
        void ShowDatePicker(bool isStart);

        /// <summary>
        /// Shows the time picker.
        /// </summary>
        void ShowTimePicker(bool isStart);

        void StartGetLocation();
    }

    public class ParkingSearchViewModel : BaseViewModel
    {        
        #region Constructors

        public ParkingSearchViewModel(IApiService apiService, ICacheService cacheService) : base(apiService, cacheService)
        {
        }

        #endregion

        #region Init

        public async void Init(SearchMode mode)
        {
            this.SearchMode = mode;

            await Task.Delay(100);

            SearchTitle = SearchMode == SearchMode.Now ? TextSource.GetText("SearchingForText") : TextSource.GetText("BookForText");

            if (mCacheService.SelectedVehicle == null)
                Debug.WriteLine("Selected Vehicle is null. Something has gone wrong @.@ !");

            Vehicle = mCacheService.SelectedVehicle;

            PlateNumber = Vehicle.PlateNumber;
        }

        #endregion

        public IParkingSearchView View {get;set;}

        #region Properties

        #region SearchMode

        private SearchMode mSearchMode = SearchMode.Now;

        public SearchMode SearchMode
        {
            get
            {
                return mSearchMode;
            }
            set
            {
                mSearchMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region SearchTitle

        private string mSearchTitle = string.Empty;

        public string SearchTitle
        {
            get
            {
                return mSearchTitle;
            }
            set
            {
                mSearchTitle = value;
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

        #region StartBookingDateTime

        private DateTime mStartBookingDateTime = DateTime.UtcNow.ToLocalTime();

        public DateTime StartBookingDateTime
        {
            get
            {
                return mStartBookingDateTime;
            }
            set
            {
                mStartBookingDateTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region EndBookingDateTime

        private DateTime mEndBookingDateTime = DateTime.UtcNow.ToLocalTime();

        public DateTime EndBookingDateTime
        {
            get
            {
                return mEndBookingDateTime;
            }
            set
            {
                mEndBookingDateTime = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region StrNumber

        private string mStrNumber = string.Empty;

        public string StrNumber
        {
            get
            {
                return mStrNumber;
            }
            set
            {
                mStrNumber = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Street

        private string mStreet = string.Empty;

        public string Street
        {
            get
            {
                return mStreet;
            }
            set
            {
                mStreet = value;
                RaisePropertyChanged();

                IsCheckVisible = Street.Length != 0;
            }
        }

        #endregion

        #region IsCheckVisible

        private bool mIsCheckVisible = false;

        public bool IsCheckVisible
        {
            get
            {
                return mIsCheckVisible;
            }
            set
            {
                mIsCheckVisible = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Places

        private ObservableCollection<HereMapView> mPlaces = new ObservableCollection<HereMapView>();

        public ObservableCollection<HereMapView> Places
        {
            get
            {
                return mPlaces;
            }
            set
            {
                mPlaces = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Vehicle

        private Vehicle mVehicle = null;

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

        #endregion

        #region Commands

        #region GotoParkingListsCommand

        private MvxCommand mGotoParkingListsCommand = null;

        public MvxCommand GotoParkingListsCommand
        {
            get
            {
                if (mGotoParkingListsCommand == null)
                {
                    mGotoParkingListsCommand = new MvxCommand(this.GotoParkingLists);
                }
                return mGotoParkingListsCommand;
            }
        }

        private void GotoParkingLists()
        {
            mCacheService.SelectedVehicle = Vehicle;
            ShowViewModel<ParkingListsViewModel>(new {startTimeStamp = StartBookingDateTime.DateTimeToTimeStamp()});
        }

        #endregion

        #region GotoChooseVehicleCommand

        private MvxCommand mGotoChooseVehicleCommand = null;

        public MvxCommand GotoChooseVehicleCommand
        {
            get
            {
                if (mGotoChooseVehicleCommand == null)
                {
                    mGotoChooseVehicleCommand = new MvxCommand(this.GotoChooseVehicle);
                }
                return mGotoChooseVehicleCommand;
            }
        }

        private void GotoChooseVehicle()
        {
            ShowViewModel<ChooseVehicleViewModel>(new {mode = ChooseVehicleMode.ReSelect});
        }

        #endregion

        #region SelectEndDateCommand

        private MvxCommand mSelectEndDateCommand = null;

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
            if (View != null)
                View.ShowDatePicker(false);
        }

        #endregion

        #region SelectEndHourCommand

        private MvxCommand mSelectEndHourCommand = null;

        public MvxCommand SelectEndHourCommand
        {
            get
            {
                if (mSelectEndHourCommand == null)
                {
                    mSelectEndHourCommand = new MvxCommand(this.SelectEndHour);
                }
                return mSelectEndHourCommand;
            }
        }

        private void SelectEndHour()
        {
            if (View != null)
                View.ShowTimePicker(false);
        }

        #endregion

        #region SelectStartDateCommand

        private MvxCommand mSelectStartDateCommand = null;

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
            if (View != null)
                View.ShowDatePicker(true);
        }

        #endregion

        #region SelectStartHourCommand

        private MvxCommand mSelectStartHourCommand = null;

        public MvxCommand SelectStartHourCommand
        {
            get
            {
                if (mSelectStartHourCommand == null)
                {
                    mSelectStartHourCommand = new MvxCommand(this.SelectStartHour);
                }
                return mSelectStartHourCommand;
            }
        }

        private void SelectStartHour()
        {
            if (View != null)
                View.ShowTimePicker(true);
        }

        #endregion

        #region CheckPlacesCommand

        private MvxCommand mCheckPlacesCommand = null;

        public MvxCommand CheckPlacesCommand
        {
            get
            {
                if (mCheckPlacesCommand == null)
                {
                    mCheckPlacesCommand = new MvxCommand(this.CheckPlaces);
                }
                return mCheckPlacesCommand;
            }
        }

        private async void CheckPlaces()
        {
            if (mCacheService.CurrentLat == 0 && mCacheService.CurrentLng == 0)
            {
                if (View != null)
                    View.StartGetLocation();

                //do get the location in view, then please call this command again, it should query the 'places'
            }
            else
            {
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, true));
                var result = await mApiService.CheckPlaces(string.Format("{0} {1}", StrNumber, Street), mCacheService.CurrentLat, mCacheService.CurrentLng);
                Places.Clear();
                if (result != null && result.Response != null && result.Response.View.Count > 0)
                {
                    Places = new ObservableCollection<HereMapView>(result.Response.View);
                }
                else
                {
                    Mvx.Resolve<IMvxMessenger>().Publish(new ToastMessage(this, "No places found!"));
                }
                Mvx.Resolve<IMvxMessenger>().Publish(new ProgressMessage(this, false));
            }
        }

        #endregion

        #region SelectPlaceCommand

        private MvxCommand<HereMapView> mSelectPlaceCommand = null;

        public MvxCommand<HereMapView> SelectPlaceCommand
        {
            get
            {
                if (mSelectPlaceCommand == null)
                {
                    mSelectPlaceCommand = new MvxCommand<HereMapView>(this.SelectPlace);
                }
                return mSelectPlaceCommand;
            }
        }

        private void SelectPlace(HereMapView hereMapView)
        {
            mCacheService.SelectedVehicle = Vehicle;
            mCacheService.SearchMode = SearchMode.Later;
            ShowViewModel<ParkingListsViewModel>(new {startTimeStamp = StartBookingDateTime.DateTimeToTimeStamp(), latitude = hereMapView.Result[0].Location.NavigationPosition[0].Latitude, longitude = hereMapView.Result[0].Location.NavigationPosition[0].Longitude});
        }

        #endregion

        #endregion

        #region Methods

        public void CloseViewModel()
        {
            Close(this);
        }
        #endregion
    }
    
}

